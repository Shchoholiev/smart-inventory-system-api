using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Common.Exceptions;
using Microsoft.Extensions.Logging;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.AdminDto;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Domain.Enums;
using SmartInventorySystemApi.Infrastructure.Services.Identity;
using DeviceEntity = SmartInventorySystemApi.Domain.Entities.Device;
using Shelf = SmartInventorySystemApi.Domain.Entities.Shelf;

namespace SmartInventorySystemApi.Infrastructure.Services;

// TODO: Add checks that user is part of the same group as devices?
public class DevicesService : ServiceBase, IDevicesService
{
    private readonly IDevicesRepository _devicesRepository;
    
    private readonly IShelvesRepository _shelvesRepository;

    // Azure IoT Hub Registry Manager
    private readonly RegistryManager _registryManager;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public DevicesService(
        IDevicesRepository devicesRepository, 
        RegistryManager registryManager,
        IShelvesRepository shelvesRepository,
        ILogger<DevicesService> logger,
        IMapper mapper)
    {
        _devicesRepository = devicesRepository;
        _registryManager = registryManager;
        _shelvesRepository = shelvesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<DeviceAdminDto> CreateDeviceAsync(DeviceCreateDto deviceCreateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating a new device with name {deviceCreateDto.Name}.");

        if (deviceCreateDto.Type == DeviceType.Unknown)
        {
            throw new MissingFieldException("Device type is required.");
        }

        var device = _mapper.Map<DeviceEntity>(deviceCreateDto);
        device.Guid = Guid.NewGuid();
        device.CreatedById = GlobalUser.Id.Value;
        device.CreatedDateUtc = DateTime.UtcNow;

        // Register the device in Azure IoT Hub
        Device iotDevice = null;
        try
        {
            iotDevice = await _registryManager.AddDeviceAsync(new Device(device.Guid.ToString()));
        }
        catch (DeviceAlreadyExistsException ex)
        {
            throw new EntityAlreadyExistsException($"Device with Id {device.Guid} already exists in Azure IoT Hub.", ex);
        }

        var createdDevice = await _devicesRepository.AddAsync(device, cancellationToken);

        switch (createdDevice.Type)
        {
            case DeviceType.Rack4ShelfController:

                _logger.LogInformation($"Creating 4 shelves for device with Id {createdDevice.Id}.");
                
                var shelves = new List<Shelf>();
                for (int i = 0; i < 4; i++)
                {
                    var shelf = new Shelf
                    {
                        DeviceId = createdDevice.Id,
                        Name = $"{createdDevice.Name} Shelf #{i + 1}",
                        CreatedById = GlobalUser.Id.Value,
                        CreatedDateUtc = DateTime.UtcNow
                    };
                    shelves.Add(shelf);
                }
                
                await _shelvesRepository.AddManyShelvesAsync(shelves, cancellationToken);

                _logger.LogInformation($"Created {shelves.Count} shelves for device with Id {createdDevice.Id}.");

                break;

            case DeviceType.Unknown:
            case DeviceType.AccessPoint:
            default:
                break;
        }

        var deviceDto = _mapper.Map<DeviceAdminDto>(createdDevice);
        deviceDto.AccessKey = iotDevice.Authentication.SymmetricKey.PrimaryKey;

        _logger.LogInformation($"Device with Id {deviceDto.Id} is created.");
        
        return deviceDto;
    }

    public async Task<DeviceDto> GetDeviceAsync(string deviceId, CancellationToken cancellationToken)
    {
        var id = ParseObjectId(deviceId);
        var device = await _devicesRepository.GetOneAsync(id, cancellationToken);
        if (device == null)
        {
            throw new EntityNotFoundException($"Device with Id {deviceId} is not found in database.");
        }

        var deviceDto = _mapper.Map<DeviceDto>(device);

        return deviceDto;
    }

    public async Task<List<DeviceDto>> GetDevicesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting a page of devices for group with Id {groupId}.");

        var groupObjectId = ParseObjectId(groupId);
        var devices = await _devicesRepository.GetPageAsync(page, size, d => d.GroupId == groupObjectId, cancellationToken);
        var deviceDtos = _mapper.Map<List<DeviceDto>>(devices);

        _logger.LogInformation($"Found {deviceDtos.Count} devices.");

        return deviceDtos;
    }

    public async Task<DeviceDto> UpdateDeviceAsync(string deviceId, DeviceUpdateDto deviceUpdateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating device with Id {deviceId}.");

        var id = ParseObjectId(deviceId);
        var device = await _devicesRepository.GetOneAsync(id, cancellationToken);
        if (device == null)
        {
            throw new EntityNotFoundException($"Device with Id {deviceId} is not found in database.");
        }

        _mapper.Map(deviceUpdateDto, device);
        device.LastModifiedById = GlobalUser.Id.Value;
        device.LastModifiedDateUtc = DateTime.UtcNow;

        var updatedDevice = await _devicesRepository.UpdateAsync(device, cancellationToken);

        var deviceDto = _mapper.Map<DeviceDto>(updatedDevice);

        _logger.LogInformation($"Device with Id {deviceId} is updated.");

        return deviceDto;
    }

    public async Task<DeviceDto> UpdateDeviceStatusAsync(string deviceId, DeviceStatusChangeDto deviceDto, CancellationToken cancellationToken)
    {
        var id = ParseObjectId(deviceId);
        var device = await _devicesRepository.GetOneAsync(id, cancellationToken);
        if (device == null)
        {
            throw new EntityNotFoundException($"Device with Id {deviceId} is not found in database.");
        }

        if (deviceDto.IsActive)
        {
            device.IsActive = true;
            var groupObjectId = ParseObjectId(deviceDto.GroupId);
            device.GroupId = groupObjectId;
        } 
        else 
        {
            throw new NotImplementedException("Deactivation of a device is not implemented.");
        }

        device.LastModifiedById = GlobalUser.Id.Value;
        device.LastModifiedDateUtc = DateTime.UtcNow;

        var updatedDevice = await _devicesRepository.UpdateAsync(device, cancellationToken);

        var deviceDtoResult = _mapper.Map<DeviceDto>(updatedDevice);

        _logger.LogInformation($"Device with Id {deviceId} is updated.");

        return deviceDtoResult;
    }
}

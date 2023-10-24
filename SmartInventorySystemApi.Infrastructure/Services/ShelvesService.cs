using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ShelvesService : ServiceBase, IShelvesService
{
    private readonly IShelvesRepository _shelvesRepository;
    
    private readonly IItemsRepository _itemsRepository;

    private readonly IDevicesRepository _devicesRepository;

    // Azure IoT Hub Service Client
    private readonly ServiceClient _serviceClient;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public ShelvesService(
        IShelvesRepository shelvesRepository, 
        IItemsRepository itemsRepository,
        IDevicesRepository devicesRepository,
        ServiceClient serviceClient,
        ILogger<DevicesService> logger,
        IMapper mapper)
    {
        _shelvesRepository = shelvesRepository;
        _itemsRepository = itemsRepository;
        _devicesRepository = devicesRepository;
        _serviceClient = serviceClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<List<ShelfDto>> GetShelvesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting shelves page {page} with size {size} for group {groupId}");

        var groupObjectId = ParseObjectId(groupId); 
        var shelves = await _shelvesRepository.GetPageAsync(page, size, s => s.GroupId == groupObjectId, cancellationToken);
        var shelfDtos = _mapper.Map<List<ShelfDto>>(shelves);

        _logger.LogInformation($"Retrieved {shelfDtos.Count} shelves");

        return shelfDtos;
    }

    public async Task<ShelfDto> GetShelfAsync(string shelfId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting shelf with Id {shelfId}");

        var id = ParseObjectId(shelfId);
        var shelf = await _shelvesRepository.GetOneAsync(id, cancellationToken);
        if (shelf == null)
        {
            throw new EntityNotFoundException($"Shelf with Id {shelfId} is not found in database.");
        }

        var shelfDto = _mapper.Map<ShelfDto>(shelf);

        _logger.LogInformation($"Retrieved shelf with Id {shelfId}");

        return shelfDto;
    }

    public async Task<List<ItemDto>> GetShelfItemsAsync(string shelfId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting items for shelf with Id {shelfId}");

        var shelfObjectId = ParseObjectId(shelfId);
        var items = await _itemsRepository.GetPageAsync(
            1, 50,
            i => i.ShelfId == shelfObjectId && i.IsDeleted == false, 
            cancellationToken);
        var itemDtos = _mapper.Map<List<ItemDto>>(items);

        _logger.LogInformation($"Retrieved {itemDtos.Count} items for shelf with Id {shelfId}");

        return itemDtos;
    }

    public async Task<ItemDto> AddItemAsync(string shelfId, ItemDto itemDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding item to shelf with Id {shelfId}");

        var shelfObjectId = ParseObjectId(shelfId);
        var item = _mapper.Map<Item>(itemDto);
        item.ShelfId = shelfObjectId;
        item.CreatedById = GlobalUser.Id.Value;
        item.CreatedDateUtc = DateTime.UtcNow;

        var createdItem = await _itemsRepository.AddAsync(item, cancellationToken);
        var dto = _mapper.Map<ItemDto>(item);

        _logger.LogInformation($"Added item with Id {createdItem.Id} to shelf with Id {shelfId}");

        return dto;
    }

    public async Task<ShelfDto> UpdateShelfAsync(string shelfId, ShelfDto shelfDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating shelf with Id {shelfId}");

        var shelfObjectId = ParseObjectId(shelfId);
        var shelf = await _shelvesRepository.GetOneAsync(shelfObjectId, cancellationToken);
        if (shelf == null)
        {
            throw new EntityNotFoundException($"Shelf with Id {shelfId} is not found in database.");
        }

        _mapper.Map(shelfDto, shelf);
        shelf.LastModifiedById = GlobalUser.Id.Value;
        shelf.LastModifiedDateUtc = DateTime.UtcNow;

        var updatedShelf = await _shelvesRepository.UpdateAsync(shelf, cancellationToken);
        var dto = _mapper.Map<ShelfDto>(updatedShelf);

        _logger.LogInformation($"Updated shelf with Id {shelfId}");

        return dto;
    }

    public async Task<ShelfDto> UpdateShelfStatusAsync(string shelfId, ShelfStatusChangeDto shelfDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating status of shelf with Id {shelfId} and item with Id {shelfDto.ItemId}");

        var itemId = ParseObjectId(shelfDto.ItemId);
        var itemTask = _itemsRepository.GetOneAsync(itemId, cancellationToken);
        
        var shelfObjectId = ParseObjectId(shelfId);
        var shelfTask = _shelvesRepository.GetOneAsync(shelfObjectId, cancellationToken);

        await Task.WhenAll(shelfTask, itemTask);

        var item = itemTask.Result;
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {shelfDto.ItemId} is not found in database.");
        }

        var shelf = shelfTask.Result;
        if (shelf == null)
        {
            throw new EntityNotFoundException($"Shelf with Id {shelfId} is not found in database.");
        }

        var device = await _devicesRepository.GetOneAsync(shelf.DeviceId, cancellationToken);
        if (device == null)
        {
            throw new EntityNotFoundException($"Shelf's Device with Id {shelf.DeviceId} is not found in database.");
        }

        await ControlLightAsync(device.Guid.ToString(), shelf.PositionInRack, shelfDto.IsLitUp, cancellationToken);

        shelf.IsLitUp = shelfDto.IsLitUp;
        shelf.LastModifiedById = GlobalUser.Id.Value;
        shelf.LastModifiedDateUtc = DateTime.UtcNow;

        var updatedShelf = await _shelvesRepository.UpdateAsync(shelf, cancellationToken);
        var dto = _mapper.Map<ShelfDto>(updatedShelf);

        _logger.LogInformation($"Updated status of shelf with Id {shelfId}");

        return dto;
    }
    
    /// <summary>
    /// Turns on/off the light of a shelf. 
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceId">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <param name="turnOn"></param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    private async Task ControlLightAsync(string deviceId, int shelfPosition, bool turnOn, CancellationToken cancellationToken)
    {
        var methodName = turnOn ? "TurnOnLight" : "TurnOffLight";
        var action = turnOn ? "on" : "off";
        var payload = new {
            shelfPosition = shelfPosition
        };
        var jsonPayload = JsonConvert.SerializeObject(payload);
        
        var methodInvocation = new CloudToDeviceMethod(methodName, TimeSpan.FromSeconds(30));
        methodInvocation.SetPayloadJson(jsonPayload);
        var response = await _serviceClient.InvokeDeviceMethodAsync(deviceId, methodInvocation, cancellationToken);
        
        if (response.Status != 200)
        {
            throw new IoTDeviceException($"Failed to turn {action} the light for shelf #{shelfPosition} for device with Id {deviceId}.");
        }
    }
}

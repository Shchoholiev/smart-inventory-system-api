using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ShelfControllersService : ServiceBase, IShelfControllersService
{
    // Azure IoT Hub Service Client
    private readonly ServiceClient _serviceClient;

    private readonly IItemHistoryRepository _itemHistoryRepository;

    private readonly IItemsRepository _itemsRepository;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public ShelfControllersService(
        ServiceClient serviceClient,
        IItemHistoryRepository itemHistoryRepository,
        IItemsRepository itemsRepository,
        ILogger<ShelvesService> logger,
        IMapper mapper)
    {
        _serviceClient = serviceClient;
        _itemHistoryRepository = itemHistoryRepository;
        _itemsRepository = itemsRepository;
        _mapper = mapper;
        _logger = logger;
    }
    
    /// <summary>
    /// Turns on/off the light of a shelf. 
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceId">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <param name="turnOn"></param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    public async Task ControlLightAsync(string deviceId, int shelfPosition, bool turnOn, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Turning {(turnOn ? "on" : "off")} the light for shelf #{shelfPosition} for Azure IoT device with DeviceId: {deviceId}.");

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

        _logger.LogInformation($"Successfully turned {action} the light for shelf #{shelfPosition} for device with Id {deviceId}.");
    }

    public async Task ControlLightAsync(string deviceId, int shelfPosition, bool turnOn, string itemId, string comment, CancellationToken cancellationToken)
    {
        await ControlLightAsync(deviceId, shelfPosition, turnOn, cancellationToken);

        _logger.LogInformation($"Saving item history for item with Id {itemId}.");

        var itemObjectId = ParseObjectId(itemId);
        var item = await _itemsRepository.GetOneAsync(itemObjectId, cancellationToken);
        if (item == null)
        {
            throw new EntityNotFoundException($"Item with Id {itemId} not found.");
        }

        var itemHistory = new ItemHistory
        {
            ItemId = itemObjectId,
            Comment = comment,
            IsTaken = item.IsTaken,
            CreatedById = GlobalUser.Id.Value,
            CreatedDateUtc = DateTime.UtcNow
        };
        await _itemHistoryRepository.AddAsync(itemHistory, cancellationToken);

        _logger.LogInformation($"Successfully saved item history for item with Id: {itemId}.");
    }
}

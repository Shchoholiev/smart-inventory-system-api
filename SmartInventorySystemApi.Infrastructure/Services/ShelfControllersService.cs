using AutoMapper;
using Microsoft.Azure.Devices;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IServices;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ShelfControllersService : IShelfControllersService
{
    // Azure IoT Hub Service Client
    private readonly ServiceClient _serviceClient;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public ShelfControllersService(
        ServiceClient serviceClient,
        ILogger<ShelvesService> logger,
        IMapper mapper)
    {
        _serviceClient = serviceClient;
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
}

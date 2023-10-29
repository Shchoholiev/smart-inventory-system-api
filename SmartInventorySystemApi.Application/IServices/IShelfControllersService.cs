namespace SmartInventorySystemApi.Application.IServices;

public interface IShelfControllersService
{
    /// <summary>
    /// Turns on/off the light of a shelf. 
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceId">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    Task ControlLightAsync(string deviceId, int shelfPosition, bool turnOn, CancellationToken cancellationToken);

    /// <summary>
    /// Turns on/off the light of a shelf and saves item history.
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceId">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    Task ControlLightAsync(string deviceId, int shelfPosition, bool turnOn, string itemId, string comment, CancellationToken cancellationToken);
}

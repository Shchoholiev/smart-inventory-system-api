using SmartInventorySystemApi.Application.Models;

namespace SmartInventorySystemApi.Application.IServices;

public interface IShelfControllersService
{
    /// <summary>
    /// Turns on/off the light of a shelf. 
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceGuid">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    Task ControlLightAsync(string deviceGuid, int shelfPosition, bool turnOn, CancellationToken cancellationToken);

    /// <summary>
    /// Turns on/off the light of a shelf and saves item history.
    /// Sends a direct method to the Azure IoT device.
    /// </summary>
    /// <param name="deviceGuid">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    /// <exception cref="IoTDeviceException">Exception is thrown when method invocation fails.</exception>
    Task ControlLightAsync(string deviceGuid, int shelfPosition, bool turnOn, string itemId, string comment, CancellationToken cancellationToken);

    /// <summary>
    /// Updates shelf 'IsLitUp' in database without triggering light.
    /// </summary>
    /// <param name="deviceGuid">Azure IoT Device Id</param>
    Task UpdateShelfStatusAsync(string deviceGuid, int shelfPosition, ShelfControllerStatusChangeDto statusChangeDto, CancellationToken cancellationToken);

    /// <summary>
    /// When movement is detected on a shelf <b>Shelf Controller</b> sends request to this endpoint.
    /// Method checks if the shelf was recently lit up as a result of <b>Access Point</b> use or manual action in application. 
    /// If it was then it turns off the light on the shelf as <b>User</b> most likely lit up a shelf to put item away.
    /// </summary>
    /// <param name="deviceGuid">Azure IoT Device Id</param>
    /// <param name="shelfPosition">Shelf position in rack. Count starts from bottom.</param>
    Task HandleMovementAsync(string deviceGuid, int shelfPosition, CancellationToken cancellationToken);
}

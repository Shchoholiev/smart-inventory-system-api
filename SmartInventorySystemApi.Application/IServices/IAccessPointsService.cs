using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.IServices;

/// <summary>
/// Service used by <b>Access Point Devices</b> and <b>Users</b>.
/// </summary>
public interface IAccessPointsService
{
    /// <summary>
    /// <list type="number">
    ///     <item>Reads QR/Bar code from an image using internal ML API. If found skip to #3.</item>
    ///     <item>Recognizes an item on the image and searchers for it in a database. If found proceedes to #3.</item>
    ///     <item>Turns on light on for the shelf where this item is located. </item>
    /// </list>
    /// All scans history is saved in a database.
    /// Used only by <b>Access Point Device</b>.
    /// </summary>
    /// <param name="deviceGuid">Device Guid -> Azure IoT DeviceId</param>
    Task FindItemByImageAsync(string deviceGuid, byte[] image, CancellationToken cancellationToken);

    Task<List<ScanHistoryDto>> GetScansHistoryAsync(int page, int size, string deviceId, CancellationToken cancellationToken);
}

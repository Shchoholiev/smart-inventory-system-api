using SmartInventorySystemApi.Application.Models.Common;

namespace SmartInventorySystemApi.Application.IServices;

public interface IImageRecognitionService
{
    /// <summary>
    /// Extract tags from an image using ML. 
    /// Currently uses Azure Cognitive Services.
    /// </summary>
    Task<IList<Tag>> GetImageTagsAsync(Stream image, CancellationToken cancellationToken);

    /// <summary>
    /// Finds and reads QR/Bar codes from an image.
    /// Currently uses internal Python ML API.
    /// </summary>
    Task<IList<ScannableCode>> ReadQrBarCodesAsync(Stream image, CancellationToken cancellationToken);
}

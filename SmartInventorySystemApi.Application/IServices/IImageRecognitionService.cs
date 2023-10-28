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
    Task<IList<ScannableCode>> ReadScannableCodeAsync(Stream image, CancellationToken cancellationToken);

    /// <summary>
    /// Generates QR/Bar codes using internal Python ML API.
    /// Barcode generation is not supported yet.
    /// </summary>
    Task<Stream> GenerateScannableCodeAsync(ScannableCode code, CancellationToken cancellationToken);
}

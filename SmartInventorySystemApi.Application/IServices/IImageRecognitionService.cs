using SmartInventorySystemApi.Application.Models;

namespace SmartInventorySystemApi.Application.IServices;

public interface IImageRecognitionService
{
    Task<IList<Tag>> GetImageTagsAsync(Stream image, CancellationToken cancellationToken);
}

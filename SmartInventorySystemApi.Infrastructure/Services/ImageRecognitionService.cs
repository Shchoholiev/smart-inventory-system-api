using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Common;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ImageRecognitionService : IImageRecognitionService
{
    private readonly IComputerVisionClient _computerVisionClient;

    public ImageRecognitionService(IComputerVisionClient computerVisionClient)
    {
        _computerVisionClient = computerVisionClient;
    }

    public async Task<IList<Tag>> GetImageTagsAsync(Stream image, CancellationToken cancellationToken)
    {
        var tagResult = await _computerVisionClient.TagImageInStreamAsync(image, cancellationToken: cancellationToken);

        // TODO: Use mapper?
        var tags = tagResult.Tags
            .Select(x => new Tag
            {
                Name = x.Name,
                Confidence = x.Confidence
            })
            .ToList();
        
        return tags;
    }

    public Task<IList<ScannableCode>> ReadQrBarCodesAsync(Stream image, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

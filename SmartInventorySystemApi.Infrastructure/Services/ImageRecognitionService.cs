using System.Net.Http.Json;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Common;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class ImageRecognitionService : IImageRecognitionService
{
    private readonly IComputerVisionClient _computerVisionClient;

    private readonly HttpClient _httpClient;

    private readonly ILogger<ImageRecognitionService> _logger;

    public ImageRecognitionService(
        IComputerVisionClient computerVisionClient,
        HttpClient httpClient,
        ILogger<ImageRecognitionService> logger)
    {
        _computerVisionClient = computerVisionClient;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<IList<Tag>> GetImageTagsAsync(Stream image, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting image tags.");

        var tagResult = await _computerVisionClient.TagImageInStreamAsync(image, cancellationToken: cancellationToken);

        // TODO: Use mapper?
        var tags = tagResult.Tags
            .Select(x => new Tag
            {
                Name = x.Name,
                Confidence = x.Confidence
            })
            .ToList();
        
        _logger.LogInformation($"Found {tags.Count} tags.");
        
        return tags;
    }

    public async Task<IList<ScannableCode>> ReadScannableCodeAsync(Stream image, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Reading scannable code from image.");

        var response = await _httpClient.PostAsync("scannable-codes/decode", new StreamContent(image), cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var scannableCodes = JsonConvert.DeserializeObject<IList<ScannableCode>>(content);

        _logger.LogInformation($"Found {scannableCodes.Count} scannable codes.");

        return scannableCodes;
    }

    public async Task<Stream> GenerateScannableCodeAsync(ScannableCode code, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Generating scannable code.");

        var response = await _httpClient.PostAsJsonAsync("scannable-codes/generate", code, cancellationToken);
        response.EnsureSuccessStatusCode();

        var generatedCode = await response.Content.ReadAsStreamAsync();

        _logger.LogInformation("Scannable code generated.");

        return generatedCode;
    }
}

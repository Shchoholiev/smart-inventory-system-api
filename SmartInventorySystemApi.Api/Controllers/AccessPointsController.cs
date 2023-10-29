using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("access-points")]
public class AccessPointsController : ApiController
{
    private readonly IAccessPointsService _accessPointsService;

    public AccessPointsController(
        IAccessPointsService accessPointsService)
    {
        _accessPointsService = accessPointsService;
    }

    [HttpPost("{deviceGuid}/items/identify-by-image")]
    public async Task<IActionResult> FindItemByImageAsync(string deviceGuid, IFormFile image, CancellationToken cancellationToken)
    {
        if (image == null || image.Length == 0)
        {
            return BadRequest("Image file is required.");
        }

        byte[] imageData;
        using (var memoryStream = new MemoryStream())
        {
            await image.CopyToAsync(memoryStream, cancellationToken);
            imageData = memoryStream.ToArray();
        }

        await _accessPointsService.FindItemByImageAsync(deviceGuid, imageData, cancellationToken);

        return Ok();
    }

    [HttpGet("{deviceId}/scans-history")]
    [Authorize]
    public async Task<List<ScanHistoryDto>> GetScansHistoryAsync(int page, int size, string deviceId, CancellationToken cancellationToken)
    {
        return await _accessPointsService.GetScansHistoryAsync(page, size, deviceId, cancellationToken);
    }
}
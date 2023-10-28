using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("access-points")]
public class AccessPointsController : ApiController
{
    private readonly IImageRecognitionService _devicesService;

    public AccessPointsController(
        IImageRecognitionService devicesService)
    {
        _devicesService = devicesService;
    }
}
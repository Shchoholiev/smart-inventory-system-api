using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;

namespace SmartInventorySystemApi.Api.Controllers;

// TODO: Add Authorization 
[Route("shelf-controllers")]
public class ShelfControllersController : ApiController
{
    private readonly IShelfControllersService _shelfControllersService;

    public ShelfControllersController(
        IShelfControllersService shelfControllersService)
    {
        _shelfControllersService = shelfControllersService;
    }

    [HttpPatch("{deviceGuid}/shelf/{shelfPosition}/status")]
    public async Task<IActionResult> UpdateShelfStatusAsync(string deviceGuid, int shelfPosition, [FromBody] ShelfControllerStatusChangeDto statusChangeDto, CancellationToken cancellationToken)
    {
        await _shelfControllersService.UpdateShelfStatusAsync(deviceGuid, shelfPosition, statusChangeDto, cancellationToken);
        return Ok();
    }

    [HttpPost("{deviceGuid}/shelf/{shelfPosition}/movements")]
    public async Task<IActionResult> HandleMovementAsync(string deviceGuid, int shelfPosition, CancellationToken cancellationToken)
    {
        await _shelfControllersService.HandleMovementAsync(deviceGuid, shelfPosition, cancellationToken);
        return Ok();
    }
}

using SmartInventorySystemApi.Application.IServices;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models;
using Microsoft.AspNetCore.Authorization;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("shelves")]
public class ShelvesController : ControllerBase
{
    private readonly IShelvesService _shelvesService;

    public ShelvesController(
        IShelvesService shelvesService)
    {
        _shelvesService = shelvesService;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<PagedList<ShelfDto>>> GetShelvesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken)
    {
        var shelves = await _shelvesService.GetShelvesPageAsync(page, size, groupId, cancellationToken);
        return Ok(shelves);
    }

    [Authorize]
    [HttpGet("{shelfId}")]
    public async Task<ActionResult<ShelfDto>> GetShelfAsync(string shelfId, CancellationToken cancellationToken)
    {
        var shelf = await _shelvesService.GetShelfAsync(shelfId, cancellationToken);
        return Ok(shelf);
    }

    [Authorize(Roles = "Owner")]
    [HttpPut("{shelfId}")]
    public async Task<ActionResult<ShelfDto>> UpdateShelfAsync(string shelfId, ShelfDto shelfDto, CancellationToken cancellationToken)
    {
        var updatedShelf = await _shelvesService.UpdateShelfAsync(shelfId, shelfDto, cancellationToken);
        return Ok(updatedShelf);
    }

    [Authorize]
    [HttpGet("{shelfId}/items")]
    public async Task<ActionResult<List<ItemDto>>> GetShelfItemsAsync(string shelfId, CancellationToken cancellationToken)
    {
        var items = await _shelvesService.GetShelfItemsAsync(shelfId, cancellationToken);
        return Ok(items);
    }

    [Authorize]
    [HttpPost("{shelfId}/items")]
    public async Task<ActionResult<ItemDto>> AddItemAsync(string shelfId, ItemDto itemDto, CancellationToken cancellationToken)
    {
        var addedItem = await _shelvesService.AddItemAsync(shelfId, itemDto, cancellationToken);
        return Ok(addedItem);
    }

    [Authorize]
    [HttpPatch("{shelfId}/status")]
    public async Task<ActionResult<ShelfDto>> UpdateShelfStatusAsync(string shelfId, ShelfStatusChangeDto shelfDto, CancellationToken cancellationToken)
    {
        var updatedShelf = await _shelvesService.UpdateShelfStatusAsync(shelfId, shelfDto, cancellationToken);
        return Ok(updatedShelf);
    }
}

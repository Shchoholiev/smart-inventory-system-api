using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Api.Controllers;

[Authorize]
[Route("items")]
public class ItemsController : ApiController
{
    private readonly IItemsService _itemsService;

    public ItemsController(IItemsService itemsService)
    {
        _itemsService = itemsService;
    }

    [HttpGet]
    public async Task<ActionResult<PagedList<ItemDto>>> GetItemsPageAsync(int page, int size, string groupId, string? search, bool? IsTaken, CancellationToken cancellationToken)
    {
        var items = await _itemsService.GetItemsPageAsync(page, size, groupId, search, IsTaken, cancellationToken);
        return Ok(items);
    }

    [HttpGet("{itemId}")]
    public async Task<ActionResult<ItemDto>> GetItemAsync(string itemId, CancellationToken cancellationToken)
    {
        var item = await _itemsService.GetItemAsync(itemId, cancellationToken);
        return Ok(item);
    }

    [HttpPut("{itemId}")]
    public async Task<ActionResult<ItemDto>> UpdateItemAsync(string itemId, [FromBody] ItemCreateDto itemDto, CancellationToken cancellationToken)
    {
        var item = await _itemsService.UpdateItemAsync(itemId, itemDto, cancellationToken);
        return Ok(item);
    }

    [HttpPatch("{itemId}/status")]
    public async Task<ActionResult<ItemDto>> UpdateItemStatusAsync(string itemId, [FromBody] ItemStatusChangeDto status, CancellationToken cancellationToken)
    {
        var item = await _itemsService.UpdateItemStatusAsync(itemId, status, cancellationToken);
        return Ok(item);
    }

    [HttpDelete("{itemId}")]
    public async Task<ActionResult> DeleteItemAsync(string itemId, CancellationToken cancellationToken)
    {
        await _itemsService.DeleteItemAsync(itemId, cancellationToken);
        return NoContent();
    }

    [HttpGet("{itemId}/history")]
    public async Task<ActionResult<List<ItemHistoryDto>>> GetItemHistoryPageAsync(string itemId, int page, int size, CancellationToken cancellationToken)
    {
        var itemHistory = await _itemsService.GetItemHistoryPageAsync(itemId, page, size, cancellationToken);
        return Ok(itemHistory);
    }
}
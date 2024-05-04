using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Application.IServices;

// TODO: Add check if user is part of the group this item belongs to.
public interface IItemsService
{
    Task<PagedList<ItemDto>> GetItemsPageAsync(
        int page, 
        int size, 
        string groupId, 
        string? search, 
        bool? IsTaken, 
        string? shelfId, 
        CancellationToken cancellationToken);

    Task<ItemDto> GetItemAsync(string itemId, CancellationToken cancellationToken);

    Task<ItemDto> UpdateItemAsync(string itemId, ItemCreateDto itemDto, CancellationToken cancellationToken);

    /// <summary>
    /// Updates Item <c>IsTaken</c> flag. And saves Item history.
    /// </summary>
    Task<ItemDto> UpdateItemStatusAsync(string itemId, ItemStatusChangeDto status, CancellationToken cancellationToken);

    Task DeleteItemAsync(string itemId, CancellationToken cancellationToken);

    Task<PagedList<ItemHistoryDto>> GetItemHistoryPageAsync(string itemId, int page, int size, CancellationToken cancellationToken);
}

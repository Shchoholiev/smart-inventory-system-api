using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Application.IServices;

public interface IShelvesService
{
    /// <summary>
    /// Returns Shelves page for a given group.
    /// User must be part of the group (Future enhancement).
    /// </summary>
    Task<PagedList<ShelfDto>> GetShelvesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a Shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<ShelfDto> GetShelfAsync(string shelfId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a Shelf.
    /// User must have role <b>Owner</b>.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<ShelfDto> UpdateShelfAsync(string shelfId, ShelfUpdateDto shelfDto, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all items that belong to the shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<List<ItemDto>> GetShelfItemsAsync(string shelfId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds an item to the shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<ItemDto> AddItemAsync(string shelfId, ItemDto itemDto, CancellationToken cancellationToken);

    // TODO: Add MoveItem Method

    /// <summary>
    /// Updates Shelf <c>IsLitUp</c> flag and turns on/off the physical light.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<ShelfDto> UpdateShelfStatusAsync(string shelfId, ShelfStatusChangeDto shelfDto, CancellationToken cancellationToken);
}

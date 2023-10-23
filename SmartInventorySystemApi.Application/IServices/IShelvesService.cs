using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IServices;

public interface IShelvesService
{
    /// <summary>
    /// Returns Shelves page for a given group.
    /// User must be part of the group (Future enhancement).
    /// </summary>
    Task<List<Shelf>> GetShelvesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Returns a Shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<Shelf> GetShelfAsync(string shelfId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a Shelf.
    /// User must have role <b>Owner</b>.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<Shelf> UpdateShelfAsync(string shelfId, Shelf shelf, CancellationToken cancellationToken);

    /// <summary>
    /// Returns all items that belong to the shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<List<Item>> GetShelfItemsAsync(string shelfId, CancellationToken cancellationToken);

    /// <summary>
    /// Adds an item to the shelf.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<Item> AddItemAsync(string shelfId, Item item, CancellationToken cancellationToken);

    // TODO: Add MoveItem Method

    /// <summary>
    /// Updates Shelf <c>IsLitUp</c> flag and turns on/off the physical light.
    /// User must be part of the group this shelf belongs to (Future enhancement).
    /// </summary>
    Task<Shelf> UpdateShelfStatusAsync(string shelfId, Shelf shelf, CancellationToken cancellationToken);
}

using MongoDB.Bson;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IShelvesRepository : IBaseRepository<Shelf>
{
    Task<Shelf> UpdateAsync(Shelf shelf, CancellationToken cancellationToken);
    
    Task<List<Shelf>> AddManyShelvesAsync(IEnumerable<Shelf> shelves, CancellationToken cancellationToken);

    Task<List<ShelfLoadLookup>> GetShelvesByItemsCountAsync(ObjectId groupId, CancellationToken cancellationToken);
}

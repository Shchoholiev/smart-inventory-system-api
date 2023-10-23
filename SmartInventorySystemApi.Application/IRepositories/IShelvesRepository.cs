using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IShelvesRepository : IBaseRepository<Shelf>
{
    Task<Shelf> UpdateAsync(Shelf shelf, CancellationToken cancellationToken);
    
    Task<List<Shelf>> AddManyShelvesAsync(IEnumerable<Shelf> shelves, CancellationToken cancellationToken);
}

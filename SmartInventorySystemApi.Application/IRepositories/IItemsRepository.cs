using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IItemsRepository : IBaseRepository<Item>
{
    Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken);
}

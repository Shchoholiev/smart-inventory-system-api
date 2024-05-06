using MongoDB.Bson;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IItemsRepository : IBaseRepository<Item>
{
    Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken);

    Task<List<ItemPopularityLookup>> GetItemsByPopularityAsync(ObjectId groupId, int count, CancellationToken cancellationToken);
    
    Task<List<UserDebtLookup>> GetUsersWithMostItemsTakenAsync(ObjectId groupId, int count, CancellationToken cancellationToken);
}

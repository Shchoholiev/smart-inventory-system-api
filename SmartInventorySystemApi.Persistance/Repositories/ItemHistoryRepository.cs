using MongoDB.Bson;
using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ItemHistoryRepository : BaseRepository<ItemHistory>, IItemHistoryRepository
{
    public ItemHistoryRepository(MongoDbContext db) : base(db, "ItemHistory") { }

    public async Task<List<ItemHistory>> GetHistoryPageAsync(int page, int size, ObjectId itemId, CancellationToken cancellationToken)
    {
        return await this._collection
            .Find(h => h.ItemId == itemId)
            .SortByDescending(h => h.CreatedDateUtc)
            .Skip((page - 1) * size)
            .Limit(size)
            .ToListAsync(cancellationToken);
    }
}

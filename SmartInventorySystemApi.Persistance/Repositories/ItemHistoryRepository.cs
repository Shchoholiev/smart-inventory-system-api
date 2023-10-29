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

    public async Task<ItemHistory> GetLatestItemHistoryInShelfAsync(ObjectId shelfId, CancellationToken cancellationToken)
    {
        // Join to Items collection to get ShelfId
        var itemHistoryLookup = new BsonDocument("$lookup",
            new BsonDocument
            {
                { "from", "Items" },
                { "let", new BsonDocument("itemId", "$ItemId") },
                { "pipeline", new BsonArray
                    {
                        new BsonDocument("$match",
                            new BsonDocument("$expr",
                                new BsonDocument("$and",
                                    new BsonArray
                                    {
                                        new BsonDocument("$eq", new BsonArray { "$_id", "$$itemId" }),
                                        new BsonDocument("$eq", new BsonArray { "$ShelfId", shelfId })
                                    }
                                )
                            )
                        )
                    }
                },
                { "as", "Item" }
            });

        var itemHistory = await _collection.Aggregate()
            .Match(h => h.CreatedDateUtc < DateTime.UtcNow.AddMinutes(-5)) // Get only history that are 5 minutes old
            .AppendStage<ItemHistory>(itemHistoryLookup)
            .SortByDescending(h => h.CreatedDateUtc)
            .FirstOrDefaultAsync(cancellationToken);

        return itemHistory;
    }
}

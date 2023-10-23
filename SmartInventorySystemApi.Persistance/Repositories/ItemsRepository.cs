using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ItemsRepository : BaseRepository<Item>, IItemsRepository
{
    public ItemsRepository(MongoDbContext db) : base(db, "Item") { }

    public async Task<Item> UpdateAsync(Item item, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Item>.Update
            .Set(i => i.Name, item.Name)
            .Set(i => i.Description, item.Description)
            .Set(i => i.IsTaken, item.IsTaken)
            .Set(i => i.LastModifiedDateUtc, item.LastModifiedDateUtc)
            .Set(i => i.LastModifiedById, item.LastModifiedById);

        var options = new FindOneAndUpdateOptions<Item>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<Item>.Filter.Eq(g => g.Id, item.Id), 
            updateDefinition, 
            options, 
            cancellationToken);
    }
}

using MongoDB.Bson;
using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ItemsRepository : BaseRepository<Item>, IItemsRepository
{
    public ItemsRepository(MongoDbContext db) : base(db, "Items") { }

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

    public async Task<List<ItemPopularityLookup>> GetItemsByPopularityAsync(ObjectId groupId, int count, CancellationToken cancellationToken)
    {
        var lookupStage = @"
            {
                $lookup: {
                from: 'ItemHistory',
                localField: '_id',
                foreignField: 'ItemId',
                as: 'history'
                }
            }";

        var addActionsCountStage = @"{
                $addFields: {
                Item: '$$ROOT',
                ActionsCount: { $size: '$history' }
                }
            }";

        var removeHistoryStage = @"{
                $project: {
                'Item.history': 0
                }
            }";

        var projectionStage = @"{
                $project: {
                    Item: 1,
                    ActionsCount: 1,
                    _id: 0
                }
            }";

        var items = await _collection
            .Aggregate()
            .Match(i => i.GroupId == groupId && !i.IsDeleted)
            .AppendStage<BsonDocument>(lookupStage)
            .AppendStage<BsonDocument>(addActionsCountStage)
            .AppendStage<BsonDocument>(removeHistoryStage)
            .AppendStage<ItemPopularityLookup>(projectionStage)
            .SortByDescending(i => i.ActionsCount)
            .Limit(count)
            .ToListAsync(cancellationToken);

        return items;
    }
}

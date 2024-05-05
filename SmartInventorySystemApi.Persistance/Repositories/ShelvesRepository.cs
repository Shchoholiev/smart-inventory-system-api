using MongoDB.Bson;
using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class ShelvesRepository: BaseRepository<Shelf>, IShelvesRepository
{
    public ShelvesRepository(MongoDbContext db) : base(db, "Shelves") { }

    public async Task<List<Shelf>> AddManyShelvesAsync(IEnumerable<Shelf> shelves, CancellationToken cancellationToken)
    {
        await _collection.InsertManyAsync(shelves, new InsertManyOptions(), cancellationToken);
        return shelves.ToList();
    }

    public Task<Shelf> UpdateAsync(Shelf shelf, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Shelf>.Update
            .Set(d => d.Name, shelf.Name)
            .Set(d => d.GroupId, shelf.GroupId)
            .Set(d => d.IsLitUp, shelf.IsLitUp)
            .Set(d => d.LastModifiedDateUtc, shelf.LastModifiedDateUtc)
            .Set(d => d.LastModifiedById, shelf.LastModifiedById);

        var options = new FindOneAndUpdateOptions<Shelf>
        {
            ReturnDocument = ReturnDocument.After
        };

        return this._collection.FindOneAndUpdateAsync(
            Builders<Shelf>.Filter.Eq(d => d.Id, shelf.Id), 
            updateDefinition, 
            options, 
            cancellationToken);
    }

    public Task<Shelf> UpdateIsLitUpAsync(Shelf shelf, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Shelf>.Update
            .Set(d => d.IsLitUp, shelf.IsLitUp)
            .Set(d => d.LastModifiedDateUtc, shelf.LastModifiedDateUtc)
            .Set(d => d.LastModifiedById, shelf.LastModifiedById);

        var options = new FindOneAndUpdateOptions<Shelf>
        {
            ReturnDocument = ReturnDocument.After
        };

        return this._collection.FindOneAndUpdateAsync(
            Builders<Shelf>.Filter.Eq(d => d.Id, shelf.Id), 
            updateDefinition, 
            options, 
            cancellationToken);
    }

    public async Task<List<ShelfLoadLookup>> GetShelvesByItemsCountAsync(ObjectId groupId, CancellationToken cancellationToken)
    {
        var lookupStage = @"
            {
                $lookup: {
                    from: 'Items', 
                    localField: '_id', 
                    foreignField: 'ShelfId',  
                    as: 'items' 
                }
            }";

        var addItemsCountStage = @"
            {
                $addFields: {
                    Shelf: '$$ROOT',
                    ItemsCount: { $size: '$items' }
                }
            }";

        var removeHistoryStage = @"
            {
                $project: {
                    'Shelf.items': 0
                }
            }";

        var projectionStage = @"
            {
                $project: {
                    Shelf: 1,
                    ItemsCount: 1,
                    _id: 0,
                }
            }";

        var shelves = await _collection
            .Aggregate()
            .Match(i => i.GroupId == groupId && !i.IsDeleted)
            .AppendStage<BsonDocument>(lookupStage)
            .AppendStage<BsonDocument>(addItemsCountStage)
            .AppendStage<BsonDocument>(removeHistoryStage)
            .AppendStage<ShelfLoadLookup>(projectionStage)
            .ToListAsync(cancellationToken);

        return shelves;
    }
}

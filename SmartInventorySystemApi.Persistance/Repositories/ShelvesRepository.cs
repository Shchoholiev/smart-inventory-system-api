using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
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
}

using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class GroupsRepository : BaseRepository<Group>, IGroupsRepository
{
    public GroupsRepository(MongoDbContext db) : base(db, "Groups") { }

    public async Task<Group> UpdateAsync(Group group, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<Group>.Update
            .Set(u => u.Name, group.Name)
            .Set(u => u.Description, group.Description)
            .Set(u => u.LastModifiedDateUtc, group.LastModifiedDateUtc)
            .Set(u => u.LastModifiedById, group.LastModifiedById);

        var options = new FindOneAndUpdateOptions<Group>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<Group>.Filter.Eq(g => g.Id, group.Id), 
            updateDefinition, 
            options, 
            cancellationToken);
    }
}

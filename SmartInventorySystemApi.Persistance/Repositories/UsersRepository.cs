using MongoDB.Bson;
using MongoDB.Driver;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities.Identity;
using SmartInventorySystemApi.Persistance.Database;

namespace SmartInventorySystemApi.Persistance.Repositories;

public class UsersRepository : BaseRepository<User>, IUsersRepository
{
    public UsersRepository(MongoDbContext db) : base(db, "Users") { }

    public async Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var updateDefinition = Builders<User>.Update
            .Set(u => u.Name, user.Name)
            .Set(u => u.Email, user.Email)
            .Set(u => u.Phone, user.Phone)
            .Set(u => u.PasswordHash, user.PasswordHash)
            .Set(u => u.GroupId, user.GroupId)
            .Set(u => u.Roles, user.Roles)
            .Set(u => u.LastModifiedDateUtc, user.LastModifiedDateUtc)
            .Set(u => u.LastModifiedById, user.LastModifiedById);

        var options = new FindOneAndUpdateOptions<User>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await this._collection.FindOneAndUpdateAsync(
            Builders<User>.Filter.Eq(u => u.Id, user.Id),
            updateDefinition,
            options,
            cancellationToken);
    }

    public async Task<List<UserActivityLookup>> GetUsersByActivityWithItemsAsync(ObjectId groupId, int count, CancellationToken cancellationToken)
    {
        var lookupStage = @"
            {
                $lookup: {
                from: 'ItemHistory',
                localField: '_id',
                foreignField: 'CreatedById',
                as: 'items',
                },
            }";

        var addActionsCountStage = @"
            {
                $addFields: {
                User: '$$ROOT',
                ActionsCount: {
                    $size: '$items',
                },
                },
            }";

        var removeHistoryStage = @"
            {
                $project: {
                'User.items': 0,
                },
            }";

        var projectionStage = @"
            {
                $project: {
                User: 1,
                ActionsCount: 1,
                _id: 0,
                },
            }";

        var users = await _collection
            .Aggregate()
            .Match(i => i.GroupId == groupId && !i.IsDeleted)
            .AppendStage<BsonDocument>(lookupStage)
            .AppendStage<BsonDocument>(addActionsCountStage)
            .AppendStage<BsonDocument>(removeHistoryStage)
            .AppendStage<UserActivityLookup>(projectionStage)
            .SortByDescending(i => i.ActionsCount)
            .Limit(count)
            .ToListAsync(cancellationToken);

        return users;
    }
}

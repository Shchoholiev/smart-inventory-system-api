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

    public async Task<List<UserDebtLookup>> GetUsersWithMostItemsTakenAsync(ObjectId groupId, int count, CancellationToken cancellationToken)
    {
        var matchStage = @"
            {
                $match: {
                IsTaken: true
                },
            }";

        var lookupStage = @"
            {
                $lookup: {
                    from: 'ItemHistory',
                    localField: '_id',
                    foreignField: 'ItemId',
                    as: 'itemsHistory',
                },
            }";

        var sortHistoryStage = @"
            {
                $sort: {
                    'itemsHistory.CreatedById': -1  
                }
            }";

        var addLatestItemHistoryStage = @"
            {
                $addFields: {
                    itemHistory: { $arrayElemAt: ['$itemsHistory', 0] },
                },
            }";

        var lookupUsersStage = @"
            {
                $lookup: {
                    from: 'Users',
                    localField: 'itemsHistory.CreatedById',
                    foreignField: '_id',
                    as: 'Users',
                },
            }";

        var addLatestUserStage = @"
            {
                $addFields: {
                    User: { $arrayElemAt: ['$Users', 0] },
                },
            }";

        var groupStage = @"
            {
                $group: {
                    _id: '$User._id',
                    User: { $first: '$User' },
                    ItemsTakenCount: { $sum: 1 }
                }
            }";

        var projectionStage = @"
            {
                $project: {
                _id: 0,
                },
            }";

        var users = await _collection
            .Aggregate()
            .Match(u => u.GroupId == groupId && !u.IsDeleted)
            .AppendStage<BsonDocument>(matchStage)
            .AppendStage<BsonDocument>(lookupStage)
            .AppendStage<BsonDocument>(sortHistoryStage)
            .AppendStage<BsonDocument>(addLatestItemHistoryStage)
            .AppendStage<BsonDocument>(lookupUsersStage)
            .AppendStage<BsonDocument>(addLatestUserStage)
            .AppendStage<BsonDocument>(groupStage)
            .AppendStage<UserDebtLookup>(projectionStage)
            .SortByDescending(i => i.ItemsTakenCount)
            .Limit(count)
            .ToListAsync(cancellationToken);

        return users;
    }
}

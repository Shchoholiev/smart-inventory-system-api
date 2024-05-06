using MongoDB.Bson;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);

    Task<List<UserActivityLookup>> GetUsersByActivityWithItemsAsync(ObjectId groupId, int count, CancellationToken cancellationToken);
}

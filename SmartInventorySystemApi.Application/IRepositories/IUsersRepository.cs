using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IUsersRepository : IBaseRepository<User>
{
    Task<User> UpdateUserAsync(User user, CancellationToken cancellationToken);
}

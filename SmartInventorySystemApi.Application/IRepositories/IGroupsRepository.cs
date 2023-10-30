using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.IRepositories;

public interface IGroupsRepository : IBaseRepository<Group>
{
    Task<Group> UpdateAsync(Group group, CancellationToken cancellationToken);
}

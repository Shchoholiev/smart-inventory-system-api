using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Application.IServices;

public interface IRolesService
{
    Task<PagedList<RoleDto>> GetRolesPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);
}

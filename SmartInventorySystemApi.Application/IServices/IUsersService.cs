using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.Application.IServices;

public interface IUsersService
{
    Task<PagedList<UserDto>> GetUsersPageAsync(int pageNumber, int pageSize, CancellationToken cancellationToken);

    Task<UserDto> GetUserAsync(string id, CancellationToken cancellationToken);
    
    Task<UserDto> GetUserByUsernameAsync(string username, CancellationToken cancellationToken);
}

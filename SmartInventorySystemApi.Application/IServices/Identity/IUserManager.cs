using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.Identity;
using SmartInventorySystemApi.Application.Models.UpdateDto;

namespace SmartInventorySystemApi.Application.IServices.Identity;

public interface IUserManager
{
    Task<TokensModel> RegisterAsync(Register register, CancellationToken cancellationToken);

    Task<TokensModel> LoginAsync(Login login, CancellationToken cancellationToken);

    Task<TokensModel> RefreshAccessTokenAsync(TokensModel tokensModel, CancellationToken cancellationToken);

    Task<UserDto> AddToRoleAsync(string roleName, string userId, CancellationToken cancellationToken);

    Task<UserDto> RemoveFromRoleAsync(string roleName, string userId, CancellationToken cancellationToken);

    Task<UpdateUserModel> UpdateAsync(UserUpdateDto userDto, CancellationToken cancellationToken);

    Task<UserDto> UpdateUserByAdminAsync(string id, UserUpdateDto userDto, CancellationToken cancellationToken);
}

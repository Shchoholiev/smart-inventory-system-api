using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.IServices;

public interface IGroupsService
{
    Task<GroupDto> CreateGroupAsync(GroupCreateDto groupCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Gets group current logged user belongs to.
    /// </summary>
    Task<GroupDto> GetCurrentGroupAsync(CancellationToken cancellationToken);

    Task<List<UserDto>> GetCurrentGroupUsersAsync(CancellationToken cancellationToken);

    Task<List<DeviceDto>> GetCurrentGroupDevicesAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Updates a group current logged user belongs to. 
    /// Only a user with <c>Owner</c> role can update a group.
    /// </summary>
    Task<GroupDto> UpdateCurrentGroupAsync(GroupCreateDto groupCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user to a group current logged user belongs to if the user is not already in the group.
    /// Only a user with <c>Owner</c> role can add users to a group.
    /// </summary>
    Task<GroupDto> AddUserToCurrentGroupAsync(string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from a group current logged user belongs to.
    /// Only a user with <c>Owner</c> role can add users to a group.
    /// </summary>
    Task<GroupDto> RemoveUserToCurrentGroupAsync(string userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Removes current user from a group current logged user belongs to.
    /// </summary>
    Task<GroupDto> LeaveGroupAsync(CancellationToken cancellationToken);
}

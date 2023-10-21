using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.IServices;

public interface IGroupsService
{
    /// <summary>
    /// Creates a new group and add current logged in user to created group.
    /// </summary>
    Task<GroupDto> CreateGroupAsync(GroupCreateDto groupCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Gets group current logged user belongs to.
    /// </summary>
    Task<GroupDto> GetGroupAsync(string groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets all users that belong to a group.
    /// </summary>
    Task<List<UserDto>> GetGroupUsersAsync(string groupId, CancellationToken cancellationToken);

    /// <summary>
    /// Updates a group. 
    /// Only a user with <c>Owner</c> role can update a group.
    /// </summary>
    Task<GroupDto> UpdateGroupAsync(string groupId, GroupCreateDto groupCreateDto, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a user to a group if the user is not already in the group.
    /// Only a user with <c>Owner</c> role can add users to a group.
    /// </summary>
    Task<GroupDto> AddUserToGroupAsync(string groupId, string userId, CancellationToken cancellationToken);

    /// <summary>
    /// Removes a user from a group.
    /// Only a user with <c>Owner</c> role can add users to a group.
    /// </summary>
    Task<GroupDto> RemoveUserFromGroupAsync(string groupId, string userId, CancellationToken cancellationToken);
    
    /// <summary>
    /// Removes current user from a group.
    /// </summary>
    Task<GroupDto> LeaveGroupAsync(string groupId, CancellationToken cancellationToken);
}

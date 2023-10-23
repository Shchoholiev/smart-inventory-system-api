using AutoMapper;
using Microsoft.Extensions.Logging;
using SmartInventorySystemApi.Application.Exceptions;
using SmartInventorySystemApi.Application.IRepositories;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.GlobalInstances;
using SmartInventorySystemApi.Domain.Entities;
using SmartInventorySystemApi.Infrastructure.Services.Identity;

namespace SmartInventorySystemApi.Infrastructure.Services;

public class GroupsService : ServiceBase, IGroupsService
{
    private readonly IGroupsRepository _groupsRepository;

    private readonly IUsersRepository _usersRepository;

    private readonly IRolesRepository _rolesRepository;

    private readonly ILogger _logger;

    private readonly IMapper _mapper;

    public GroupsService(
        IGroupsRepository groupsRepository, 
        IUsersRepository usersRepository, 
        IRolesRepository rolesRepository,
        ILogger<GroupsService> logger,
        IMapper mapper)
    {
        _groupsRepository = groupsRepository;
        _usersRepository = usersRepository;
        _rolesRepository = rolesRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GroupDto> GetGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting group with Id {groupId} by user with Id: {GlobalUser.Id}");

        var id = ParseObjectId(groupId);
        var userBelongsToGroupTask = _usersRepository.ExistsAsync(
            u => u.Id == GlobalUser.Id.Value && u.GroupId == id, cancellationToken);
        var groupTask = _groupsRepository.GetOneAsync(id, cancellationToken);

        await Task.WhenAll(userBelongsToGroupTask, groupTask);

        var userBelongsToGroup = await userBelongsToGroupTask;
        var group = await groupTask;
        if (group == null)
        {
            throw new EntityNotFoundException("Group");
        }

        if (!userBelongsToGroup)
        {
            throw new UnauthorizedAccessException("User does not belong to this group.");
        }

        var dto = _mapper.Map<GroupDto>(group);
        
        _logger.LogInformation($"Returning group with Id {groupId} for user with Id: {GlobalUser.Id}");

        return dto;
    }

    // TOOD: Add pagination
    public async Task<List<UserDto>> GetGroupUsersAsync(string groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Getting users for group with Id {groupId} by user with Id: {GlobalUser.Id}");

        var id = ParseObjectId(groupId);
        var groupExistsTask = _groupsRepository.ExistsAsync(
            g => g.Id == id, cancellationToken);
        var userBelongsToGroupTask = _usersRepository.ExistsAsync(
            u => u.Id == GlobalUser.Id.Value && u.GroupId == id, cancellationToken);
        var usersTask = _usersRepository.GetPageAsync(1, 30, u => u.GroupId == id, cancellationToken);

        await Task.WhenAll(groupExistsTask, userBelongsToGroupTask, usersTask);

        var groupExists = await groupExistsTask;
        if (!groupExists)
        {
            throw new EntityNotFoundException($"Device with Id {groupId} is not found in database.");
        }

        var userBelongsToGroup = await userBelongsToGroupTask;
        if (!userBelongsToGroup)
        {
            throw new UnauthorizedAccessException("User does not belong to this group.");
        }

        var users = await usersTask;
        var dtos = _mapper.Map<List<UserDto>>(users);

        _logger.LogInformation($"Returning {dtos.Count} users for group with Id {groupId} by user with Id: {GlobalUser.Id}");

        return dtos;
    }

    public async Task<GroupDto> CreateGroupAsync(GroupCreateDto groupCreateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Creating group for user with Id: {GlobalUser.Id}");

        var group = _mapper.Map<Group>(groupCreateDto);
        group.CreatedById = GlobalUser.Id.Value;
        group.CreatedDateUtc = DateTime.UtcNow;

        var createdGroupTask = _groupsRepository.AddAsync(group, cancellationToken);
        var currentUserTask = _usersRepository.GetOneAsync(GlobalUser.Id.Value, cancellationToken);
        var ownerRoleTask = _rolesRepository.GetOneAsync(r => r.Name == "Owner", cancellationToken);

        await Task.WhenAll(createdGroupTask, currentUserTask, ownerRoleTask);

        var createdGroup = await createdGroupTask;
        var currentUser = await currentUserTask;
        var ownerRole = await ownerRoleTask;

        currentUser.GroupId = group.Id;
        currentUser.Roles.Add(ownerRole);
        await _usersRepository.UpdateUserAsync(currentUser, cancellationToken);

        var dto = _mapper.Map<GroupDto>(createdGroup);

        _logger.LogInformation($"Returning created group by user with Id: {GlobalUser.Id}");

        return dto;
    }

    public async Task<GroupDto> UpdateGroupAsync(string groupId, GroupCreateDto groupCreateDto, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Updating group with Id: {groupId} for user with Id: {GlobalUser.Id}");

        var id = ParseObjectId(groupId);
        var groupTask = _groupsRepository.GetOneAsync(id, cancellationToken);
        var currentUserTask = _usersRepository.GetOneAsync(GlobalUser.Id.Value, cancellationToken);

        await Task.WhenAll(groupTask, currentUserTask);

        var group = groupTask.Result;
        if (group == null)
        {
            throw new EntityNotFoundException($"Group with Id: {groupId} was not found.");
        }

        var currentUser = currentUserTask.Result;
        if (currentUser.GroupId != id || !currentUser.Roles.Any(r => r.Name == "Owner"))
        {
            throw new UnauthorizedAccessException($"Current user is not authorized to update group with Id: {groupId}.");
        }

        _mapper.Map(groupCreateDto, group);
        group.LastModifiedById = GlobalUser.Id.Value;
        group.LastModifiedDateUtc = DateTime.UtcNow;

        await _groupsRepository.UpdateAsync(group, cancellationToken);

        var dto = _mapper.Map<GroupDto>(group);

        _logger.LogInformation($"Returning updated group with Id: {groupId} by user with Id: {GlobalUser.Id}");

        return dto;
    }

    public async Task<GroupDto> AddUserToGroupAsync(string groupId, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Adding user with Id: {userId} to group with Id: {groupId} by user with Id: {GlobalUser.Id}");

        var groupTask = _groupsRepository.GetOneAsync(ParseObjectId(groupId), cancellationToken);
        var userTask = _usersRepository.GetOneAsync(ParseObjectId(userId), cancellationToken);
        var currentUserTask = _usersRepository.GetOneAsync(GlobalUser.Id.Value, cancellationToken);

        await Task.WhenAll(groupTask, userTask, currentUserTask);

        var group = groupTask.Result;
        if (group == null)
        {
            throw new EntityNotFoundException($"Group with Id: {groupId} was not found.");
        }

        var user = userTask.Result;
        if (user == null)
        {
            throw new EntityNotFoundException($"User with Id: {userId} was not found.");
        }

        if (user.GroupId.HasValue)
        {
            throw new InvalidOperationException($"User with Id: {userId} is already a member of a group.");
        }

        var currentUser = currentUserTask.Result;
        if (currentUser.GroupId != group.Id || !currentUser.Roles.Any(r => r.Name == "Owner"))
        {
            throw new UnauthorizedAccessException($"Current user is not authorized to add user with Id: {userId} to group with Id: {groupId}.");
        }

        user.GroupId = group.Id;
        // Remove Owner role if user has it as now he is member of a new group
        var ownerRole = user.Roles.FirstOrDefault(r => r.Name == "Owner");
        if (ownerRole != null)
        {
            user.Roles.Remove(ownerRole);
        }
        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        var dto = _mapper.Map<GroupDto>(group);

        _logger.LogInformation($"Returning group with Id: {groupId} after adding user with Id: {userId} by user with Id: {GlobalUser.Id}");

        return dto;
    }

    public async Task<GroupDto> RemoveUserFromGroupAsync(string groupId, string userId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Removing user with Id: {userId} from group with Id: {groupId} by user with Id: {GlobalUser.Id}");

        var groupTask = _groupsRepository.GetOneAsync(ParseObjectId(groupId), cancellationToken);
        var userTask = _usersRepository.GetOneAsync(ParseObjectId(userId), cancellationToken);
        var currentUserTask = _usersRepository.GetOneAsync(GlobalUser.Id.Value, cancellationToken);

        await Task.WhenAll(groupTask, userTask, currentUserTask);

        var group = groupTask.Result;
        if (group == null)
        {
            throw new EntityNotFoundException($"Group with Id: {groupId} was not found.");
        }

        var user = userTask.Result;
        if (user == null)
        {
            throw new EntityNotFoundException($"User with Id: {userId} was not found.");
        }

        if (user.GroupId != group.Id)
        {
            throw new InvalidOperationException($"User with Id: {userId} is not a member of group with Id: {groupId}.");
        }

        var currentUser = currentUserTask.Result;
        if (currentUser.GroupId != group.Id || !currentUser.Roles.Any(r => r.Name == "Owner"))
        {
            throw new UnauthorizedAccessException($"Current user is not authorized to remove user with Id: {userId} from group with Id: {groupId}.");
        }

        user.GroupId = null;
        // Remove Owner role if user has it as he is being removed from a group
        var ownerRole = user.Roles.FirstOrDefault(r => r.Name == "Owner");
        if (ownerRole != null)
        {
            user.Roles.Remove(ownerRole);
        }
        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        var dto = _mapper.Map<GroupDto>(group);

        _logger.LogInformation($"Returning group with Id: {groupId} after removing user with Id: {userId} by user with Id: {GlobalUser.Id}");

        return dto;
    }

    public async Task LeaveGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Leaving group with Id: {groupId} by user with Id: {GlobalUser.Id}");

        var groupTask = _groupsRepository.GetOneAsync(ParseObjectId(groupId), cancellationToken);
        var userTask = _usersRepository.GetOneAsync(GlobalUser.Id.Value, cancellationToken);

        await Task.WhenAll(groupTask, userTask);

        var group = groupTask.Result;
        if (group == null)
        {
            throw new EntityNotFoundException($"Group with Id: {groupId} was not found.");
        }

        var user = userTask.Result;
        if (user == null)
        {
            throw new EntityNotFoundException($"User with Id: {GlobalUser.Id} was not found.");
        }

        if (user.GroupId != group.Id)
        {
            throw new InvalidOperationException($"User with Id: {GlobalUser.Id} is not a member of group with Id: {groupId}.");
        }

        user.GroupId = null;
        // Remove Owner role if user has it as he is leaving the group
        var ownerRole = user.Roles.FirstOrDefault(r => r.Name == "Owner");
        if (ownerRole != null)
        {
            user.Roles.Remove(ownerRole);
        }
        await _usersRepository.UpdateUserAsync(user, cancellationToken);

        _logger.LogInformation($"User with Id: {GlobalUser.Id} left group with Id: {groupId}");
    }
}

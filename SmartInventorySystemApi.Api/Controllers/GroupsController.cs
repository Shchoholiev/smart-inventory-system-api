using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("groups")]
public class GroupsController : ApiController
{
    private readonly IGroupsService _groupsService;

    public GroupsController(
        IGroupsService groupsService)
    {
        _groupsService = groupsService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateGroupAsync(GroupCreateDto groupCreateDto, CancellationToken cancellationToken)
    {
        var group = await _groupsService.CreateGroupAsync(groupCreateDto, cancellationToken);
        return CreatedAtAction(nameof(GetGroupAsync), new { groupId = group.Id }, group);
    }

    [Authorize]
    [HttpGet("{groupId}")]
    public async Task<ActionResult<GroupDto>> GetGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        var group = await _groupsService.GetGroupAsync(groupId, cancellationToken);
        return Ok(group);
    }

    [Authorize]
    [HttpGet("{groupId}/users")]
    public async Task<ActionResult<List<UserDto>>> GetGroupUsersAsync(string groupId, CancellationToken cancellationToken)
    {
        var users = await _groupsService.GetGroupUsersAsync(groupId, cancellationToken);
        return Ok(users);
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPut("{groupId}")]
    public async Task<ActionResult<GroupDto>> UpdateGroupAsync(string groupId, GroupCreateDto groupCreateDto, CancellationToken cancellationToken)
    {
        var group = await _groupsService.UpdateGroupAsync(groupId, groupCreateDto, cancellationToken);
        return Ok(group);
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpPost("{groupId}/users/{userId}")]
    public async Task<ActionResult<GroupDto>> AddUserToGroupAsync(string groupId, string userId, CancellationToken cancellationToken)
    {
        var group = await _groupsService.AddUserToGroupAsync(groupId, userId, cancellationToken);
        return Ok(group);
    }

    [Authorize(Roles = "Owner,Admin")]
    [HttpDelete("{groupId}/users/{userId}")]
    public async Task<ActionResult<GroupDto>> RemoveUserFromGroupAsync(string groupId, string userId, CancellationToken cancellationToken)
    {
        var group = await _groupsService.RemoveUserFromGroupAsync(groupId, userId, cancellationToken);
        return Ok(group);
    }

    [Authorize]
    [HttpDelete("{groupId}/users")]
    public async Task<ActionResult> LeaveGroupAsync(string groupId, CancellationToken cancellationToken)
    {
        await _groupsService.LeaveGroupAsync(groupId, cancellationToken);
        return NoContent();
    }
}
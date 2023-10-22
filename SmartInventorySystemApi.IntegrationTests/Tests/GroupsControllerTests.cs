using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.ExceptionHandling;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class GroupsControllerTests : TestsBase
{
    public GroupsControllerTests(TestingFactory<Program> factory)
        : base(factory, "groups")
    { }

    [Fact]
    public async Task GetGroupAsync_UserBelongsToGroup_ReturnsGroupDto()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";
        
        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{groupId}");
        var groupDto = await response.Content.ReadFromJsonAsync<GroupDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(groupDto);
    }

    [Fact]
    public async Task GetGroupAsync_GroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "651c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{nonExistingGroupId}");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task GetGroupAsync_UserDoesNotBelongToGroup_ReturnsForbidden()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var anotherGroupId = "662c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{anotherGroupId}");
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task GetGroupUsersAsync_UserBelongsToGroup_ReturnsUserDtos()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";
        
        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{groupId}/users");
        var userDtos = await response.Content.ReadFromJsonAsync<List<UserDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(userDtos);
        Assert.NotEmpty(userDtos);
    }

    [Fact]
    public async Task GetGroupUsersAsync_GroupDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "651c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{nonExistingGroupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetGroupUsersAsync_UserDoesNotBelongToGroup_ReturnsForbidden()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var anotherGroupId = "662c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{anotherGroupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroupAsync_ValidInput_ReturnsCreatedGroupDto()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var groupCreateDto = new GroupCreateDto
        {
            Name = "New Group",
            Description = "new group"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", groupCreateDto);
        var createdGroup = await response.Content.ReadFromJsonAsync<GroupDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdGroup);
        Assert.Equal(groupCreateDto.Name, createdGroup.Name);
    }

    [Fact]
    public async Task CreateGroupAsync_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var invalidGroupCreateDto = new GroupCreateDto
        {
            // Missing description == invalid
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", invalidGroupCreateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroupAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        // No login is done, so the user should be unauthorized
        var groupCreateDto = new GroupCreateDto
        {
            Name = "New Group",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", groupCreateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #region UpdateGroupAsync

    [Fact]
    public async Task UpdateGroupAsync_ValidInput_ReturnsUpdatedGroup()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Assuming this group ID exists and is owned by the logged-in user
        var updatedGroupDto = new GroupCreateDto
        {
            Name = "Test Group 1",
            Description = "Update group test"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{groupId}", updatedGroupDto);
        var updatedGroup = await response.Content.ReadFromJsonAsync<GroupDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedGroup);
        Assert.Equal("Update group test", updatedGroup.Description);
    }

    [Fact]
    public async Task UpdateGroupAsync_InvalidInput_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc"; 
        var invalidGroupUpdateDto = new GroupCreateDto
        {
            // Missing description == invalid
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{groupId}", invalidGroupUpdateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroupAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        // No login is done, so the user should be unauthorized
        var groupId = "652c3b89ae02a3135d6429fc";
        var groupUpdateDto = new GroupCreateDto
        {
            Name = "Unauthorized Test",
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{groupId}", groupUpdateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroupAsync_NonExistingGroup_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "751c3b89ae02a3135d6429fc";  // Assuming this group ID does not exist
        var groupUpdateDto = new GroupCreateDto
        {
            Name = "Non-existing Group",
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{nonExistingGroupId}", groupUpdateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region AddUserToGroupAsync

    [Fact]
    public async Task AddUserToGroupAsync_ValidInput_ReturnsUpdatedGroup()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID owned by the logged-in user
        var userId = "652c3b89ae02a3135d6409fc";  // Existing user ID who is not part of any group (test@gmail.com)

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{groupId}/users/{userId}", new {});
        var updatedGroup = await response.Content.ReadFromJsonAsync<GroupDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedGroup);
        // Additional checks for the updated group could be here
    }

    [Fact]
    public async Task AddUserToGroupAsync_NonExistingGroup_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "653c2b88ae00a4123d6429ff";
        var userId = "652c3b89ae02a3135d6409fc";

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{nonExistingGroupId}/users/{userId}", new {});

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroupAsync_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID
        var nonExistingUserId = "653c2b88ae00a4123d6429ff";

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{groupId}/users/{nonExistingUserId}", new {});

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroupAsync_UserAlreadyInGroup_ReturnsConflict()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID
        var userId = "652c3b89ae02a3135d6419fc";  // User ID who is already part of the group

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{groupId}/users/{userId}", new {});

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task AddUserToGroupAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        // No login is done, so the user should be unauthorized
        var groupId = "652c3b89ae02a3135d6429fc";
        var userId = "652c3b89ae02a3135d6409fc";  // Existing user ID

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{groupId}/users/{userId}", new {});

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region RemoveUserFromGroupAsync

    [Fact]
    public async Task RemoveUserFromGroupAsync_ValidInput_ReturnsUpdatedGroup()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345"); // Assumes the owner has an existing account
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID owned by the logged-in user
        var userId = "652c3b89ae02a3135d6439fc";  // Existing user ID who is part of the group (group@gmail.com)

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users/{userId}");
        var updatedGroup = await response.Content.ReadFromJsonAsync<GroupDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedGroup);
        // Additional checks for the updated group could be here
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_NonExistingGroup_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "653c2b88ae00a4123d1429ff";
        var userId = "652c3b89ae02a3135d6439fc";   // Existing user ID who is part of the group (group@gmail.com)

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{nonExistingGroupId}/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_NonExistingUser_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID
        var nonExistingUserId = "653c2b88ae00a4123d2429ff";

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users/{nonExistingUserId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_UserNotInGroup_ReturnsConflict()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID
        var userId = "652c3b89ae02a3135d6409fc";  // User ID who is not part of the group

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroupAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        // No login is done, so the user should be unauthorized
        var groupId = "652c3b89ae02a3135d6429fc";
        var userId = "652c3b89ae02a3135d6439fc";  // Existing user ID

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users/{userId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region LeaveGroupAsync

    [Fact]
    public async Task LeaveGroupAsync_ValidInput_UserLeavesGroup()
    {
        // Arrange
        await LoginAsync("group2@gmail.com", "Yuiop12345"); // User is part of a group
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID the user is part of

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task LeaveGroupAsync_NonExistingGroup_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group2@gmail.com", "Yuiop12345");
        var nonExistingGroupId = "623c2b88ae00a4123d6429ff"; 

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{nonExistingGroupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LeaveGroupAsync_UserNotInGroup_ReturnsConflict()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID that the user is not part of

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task LeaveGroupAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        // No login is done, so the user should be unauthorized
        var groupId = "652c3b89ae02a3135d6429fc";  // Existing group ID

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{groupId}/users");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}

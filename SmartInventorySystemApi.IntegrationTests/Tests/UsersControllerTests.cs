using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.ExceptionHandling;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.Identity;
using SmartInventorySystemApi.Application.Models.UpdateDto;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class UsersControllerTests : TestsBase
{
    public UsersControllerTests(TestingFactory<Program> factory)
        : base(factory, "users")
    { }

    [Fact]
    public async Task RegisterAsync_ValidInput_ReturnsTokens()
    {
        // Arrange
        var register = new Register
        {
            Email = "register@gmail.com",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tokens);
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task RegisterAsync_ExistingEmail_Returns409Conflict()
    {
        // Arrange
        var register = new Register
        {
            Email = "test@gmail.com",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task RegisterAsync_ExistingPhone_Returns409Conflict()
    {
        // Arrange
        var register = new Register
        {
            Phone = "+12345678901",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task RegisterAsync_InvalidEmail_Returns422UnprocessableEntity()
    {
        // Arrange
        var register = new Register
        {
            Email = "gmail.com",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task RegisterAsync_InvalidPhone_Returns422UnprocessableEntity()
    {
        // Arrange
        var register = new Register
        {
            Phone = "104495",
            Password = "Yuiop12345",
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/register", register);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_ValidInput_ReturnsTokens()
    {
        // Arrange
        var login = new Login
        {
            Email = "test@gmail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(tokens);
        Assert.NotNull(tokens.AccessToken);
        Assert.NotNull(tokens.RefreshToken);
    }

    [Fact]
    public async Task LoginAsync_IncorrectPassword_Returns400BadRequest()
    {
        // Arrange
        var login = new Login
        {
            Email = "test@gmail.com",
            Password = "incorrect"
        };

            // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidEmail_Returns422UnprocessableEntity()
    {
        // Arrange
        var login = new Login
        {
            Email = "mail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_InvalidPhone_Returns422UnprocessableEntity()
    {
        // Arrange
        var login = new Login
        {
            Phone = "104495",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_NonExistingPhone_Returns404NotFound()
    {
        // Arrange
        var login = new Login
        {
            Phone = "+12345678900",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task LoginAsync_NonExistingEmail_Returns404NotFound()
    {
        // Arrange
        var login = new Login
        {
            Email = "notFound@gmail.com",
            Password = "Yuiop12345"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/login", login);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task UpdateAsync_ValidRequest_ReturnsUpdatedUser()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var userDto = new UserDto
        {
            Name = "Interation test",
            Email = "test@gmail.com"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}", userDto);
        var updateUserModel = await response.Content.ReadFromJsonAsync<UpdateUserModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updateUserModel);
        Assert.NotNull(updateUserModel.User);
        Assert.NotNull(updateUserModel.Tokens);
        Assert.NotNull(updateUserModel.Tokens.AccessToken);
        Assert.NotNull(updateUserModel.Tokens.RefreshToken);
        Assert.Equal(userDto.Name, updateUserModel.User.Name);
        Assert.Equal(userDto.Email, updateUserModel.User.Email);
    }

    [Fact]
    public async Task UpdateAsync_Unauthorized_Returns401Unauthorized()
    {
        // Arrange
        var userDto = new UserDto
        {
            Email = "newEmail@gmail.com"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}", userDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UpdateAsync_InvalidRequest_Returns422UnprocessableEntity()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var userDto = new UserDto
        {
            Name = "Random",
            Email = "invalid.mail"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}", userDto);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task UpdateUserByAdminAsync_ValidRequest_ReturnsUpdatedUser()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "652c3b89ae02a3135d6309fc";
        var userDto = new UserUpdateDto
        {
            Name = "Updated Name",
            Email = "updated_email@gmail.com",
            Phone = "+1234567890"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{userId}", userDto);
        var updatedUserDto = await response.Content.ReadFromJsonAsync<UserDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedUserDto);
        Assert.Equal(userDto.Name, updatedUserDto.Name);
        Assert.Equal(userDto.Email, updatedUserDto.Email);
        Assert.Equal(userDto.Phone, updatedUserDto.Phone);
    }

    [Fact]
    public async Task UpdateUserByAdminAsync_UserNotFound_Returns404NotFound()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "652c3b79ae02a3135d6309fc";
        var userDto = new UserUpdateDto();

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{userId}", userDto);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }

    [Fact]
    public async Task UpdateUserByAdminAsync_InvalidRequest_Returns422UnprocessableEntity()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var userId = "652c3b89ae02a3135d6309fc";
        var userDto = new UserUpdateDto
        {
            Email = "invalid-email",
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{userId}", userDto);
        var error = await response.Content.ReadFromJsonAsync<ErrorResponse>();

        // Assert
        Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        Assert.NotNull(error);
        Assert.NotNull(error.Message);
    }
}
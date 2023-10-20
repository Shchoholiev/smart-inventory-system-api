using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models.Identity;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class TokensControllerTests : TestsBase
{
    public TokensControllerTests(TestingFactory<Program> factory)
        : base(factory, "tokens")
    { }

    [Fact]
    public async Task RefreshAccessTokenAsync_ValidInput_Returns200NewTokens()
    {
        // Arrange
        var initialTokens = await GetTestTokensAsync();

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/refresh", initialTokens);
        var refreshedTokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(refreshedTokens);
        Assert.NotNull(refreshedTokens.AccessToken);
        Assert.NotNull(refreshedTokens.RefreshToken);
        Assert.NotEqual(initialTokens.AccessToken, refreshedTokens.AccessToken);
        Assert.Equal(initialTokens.RefreshToken, refreshedTokens.RefreshToken);
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_InvalidAccessToken_Returns401Unauthorized()
    {
        // Arrange
        var initialTokens = await GetTestTokensAsync();
        initialTokens.AccessToken = "InvalidAccessToken";

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/refresh", initialTokens);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task RefreshAccessTokenAsync_ExpiredRefreshToken_Returns401Unauthorized()
    {
        // Arrange
        var initialTokens = await GetTestTokensAsync();
        // Manually set RefreshToken to an expired one if needed
        initialTokens.RefreshToken = "test-refresh-token";

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/refresh", initialTokens);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    private async Task<TokensModel> GetTestTokensAsync()
    {
        var login = new Login
        {
            Email = "test@gmail.com",
            Password = "Yuiop12345"
        };

        var response = await HttpClient.PostAsJsonAsync($"users/login", login);
        var tokens = await response.Content.ReadFromJsonAsync<TokensModel>();

        return tokens;
    }
}

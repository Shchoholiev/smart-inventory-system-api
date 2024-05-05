using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models.Statistics;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class StatisticsControllerTests : TestsBase
{
    public StatisticsControllerTests(TestingFactory<Program> factory)
        : base(factory, "")
    { }

    [Fact]
    public async Task GetItemsByPopularityAsync_ValidInput_Returns200()
    {
        // Arrange
        string groupId = "652c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"items/popularity?groupId={groupId}");
        var items = await response.Content.ReadFromJsonAsync<List<ItemPopularity>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(items);
    }

    [Fact]
    public async Task GetShelvesByItemsCountAsync_ValidInput_Returns200()
    {
        // Arrange
        string groupId = "652c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"shelves/items-count?groupId={groupId}");
        var shelves = await response.Content.ReadFromJsonAsync<List<ShelfLoad>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(shelves);
    }

    [Fact]
    public async Task GetUsersByActivityWithItemsAsync_ValidInput_Returns200()
    {
        // Arrange
        string groupId = "652c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"users/activity?groupId={groupId}");
        var users = await response.Content.ReadFromJsonAsync<List<UserActivity>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(users);
    }

    [Fact]
    public async Task GetUsersWithMostItemsTakenAsync_ValidInput_Returns200()
    {
        // Arrange
        string groupId = "652c3b89ae02a3135d6429fc";

        // Act
        var response = await HttpClient.GetAsync($"users/items-taken?groupId={groupId}");
        var users = await response.Content.ReadFromJsonAsync<List<UserDebt>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(users);
    }
}


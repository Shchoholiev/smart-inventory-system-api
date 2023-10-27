using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class ItemsControllerTests : TestsBase
{
    public ItemsControllerTests(TestingFactory<Program> factory)
        : base(factory, "items")
    { }

    #region GetItemsPageAsync

    [Fact]
    public async Task GetItemsPageAsync_GroupWithItems_ReturnsItemsPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        int page = 1;
        int size = 10;
        string groupId = "652c3b89ae02a3135d6429fc"; // group with items

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");
        var itemsList = await response.Content.ReadFromJsonAsync<PagedList<ItemDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemsList);
        Assert.True(itemsList.Items.Count() > 0);
    }

    [Fact]
    public async Task GetItemsPageAsync_GroupWithItemsAndIsTakenFilter_ReturnsFilteredItemsPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        var page = 1;
        var size = 10;
        var groupId = "652c3b89ae02a3135d6429fc"; // group with items
        var isTaken = true;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}&IsTaken={isTaken}");
        var itemsList = await response.Content.ReadFromJsonAsync<PagedList<ItemDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemsList);
        Assert.True(itemsList.Items.All(i => i.IsTaken == isTaken));
    }

    [Fact]
    public async Task GetItemsPageAsync_GroupWithItemsAndIsTakenAndSearch_ReturnsFilteredItemsPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        var page = 1;
        var size = 10;
        var groupId = "652c3b89ae02a3135d6429fc"; // group with items
        var search = "Item 1";
        var isTaken = false;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}&IsTaken={isTaken}&search={search}");
        var itemsList = await response.Content.ReadFromJsonAsync<PagedList<ItemDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemsList);
        Assert.True(itemsList.Items.All(i => i.IsTaken == isTaken));
        Assert.True(itemsList.Items.All(i => i.Name.Contains(search)));
    }

    [Fact]
    public async Task GetItemsPageAsync_UnauthorizedUser_ReturnsUnathorized()
    {
        // Arrange
        var page = 1;
        var size = 10;
        var groupId = "652c3b89ae02a3135d6429fc"; // group with shelves

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion


    #region GetItemAsync

    [Fact]
    public async Task GetItemAsync_ValidItemId_ReturnsItemDto()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b01ae02a3135d6439fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{itemId}");
        var itemDto = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemDto);
        Assert.Equal(itemId, itemDto.Id);
        Assert.NotNull(itemDto.Name);
        Assert.NotNull(itemDto.Description);
        Assert.NotNull(itemDto.IsTaken);
    }

    [Fact]
    public async Task GetItemAsync_InvalidItemId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b01ae02a3135d6439cc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{itemId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UpdateItemAsync

    [Fact]
    public async Task UpdateItemAsync_ValidItemIdAndDto_ReturnsUpdatedItem()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b02ae02a3135d6439fc";
        var updateDto = new ItemCreateDto
        {
            Name = "Updated name",
            Description = "Updated description",
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{itemId}", updateDto);
        var updatedItemDto = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedItemDto);
        Assert.Equal(itemId, updatedItemDto.Id);
        Assert.Equal("Updated name", updatedItemDto.Name);
        Assert.Equal("Updated description", updatedItemDto.Description);
    }

    [Fact]
    public async Task UpdateItemAsync_InvalidItemId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b02ae02a9191d6439fc"; // invalid id
        var updateDto = new ItemCreateDto
        {
            Name = "Updated name",
            Description = "Updated description",
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{itemId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region UpdateItemStatusAsync

    [Fact]
    public async Task UpdateItemStatusAsync_ValidItemIdAndStatus_ChangesStatusAndReturnsUpdatedItem()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b03ae02a3135d6439fc"; // valid id
        var statusChangeDto = new ItemStatusChangeDto
        {
            IsTaken = true,
            Comment = "Taken for maintenance"
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{itemId}/status", statusChangeDto);
        var updatedItemDto = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedItemDto);
        Assert.Equal(itemId, updatedItemDto.Id);
        Assert.True(updatedItemDto.IsTaken);
    }


    [Fact]
    public async Task UpdateItemStatusAsync_ValidItemIdAndStatusToFalse_ChangesStatusAndReturnsUpdatedItem()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b03ae02a3135d6439fc"; // valid id
        var statusChangeDto = new ItemStatusChangeDto
        {
            IsTaken = false,
            Comment = "Returned"
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{itemId}/status", statusChangeDto);
        var updatedItemDto = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedItemDto);
        Assert.Equal(itemId, updatedItemDto.Id);
        Assert.False(updatedItemDto.IsTaken);
    }

    [Fact]
    public async Task UpdateItemStatusAsync_NonExistingItemId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b03ae02a3135d6488fc"; // invalid id
        var statusChangeDto = new ItemStatusChangeDto
        {
            IsTaken = true,
            Comment = "Taken for maintenance"
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{itemId}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateItemStatusAsync_UnauthorizedUser_Unauthorized()
    {
        // Arrange
        string itemId = "651c1b03ae02a3135d6439fc"; // valid id
        var statusChangeDto = new ItemStatusChangeDto
        {
            IsTaken = true,
            Comment = "Taken for maintenance"
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{itemId}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region DeleteItemAsync

    [Fact]
    public async Task DeleteItemAsync_ValidItemId_ItemDeleted()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b04ae02a3135d6439fc"; // valid id

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{itemId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

        // Optional: You can additionally fetch the item here to make sure it's actually deleted
        var fetchResponse = await HttpClient.GetAsync($"{ResourceUrl}/{itemId}");
        Assert.Equal(HttpStatusCode.NotFound, fetchResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteItemAsync_NonExistingItemId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string itemId = "651c1b24ae02a3135d6439fc"; // non existing id

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{itemId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteItemAsync_UnauthorizedUser_Unauthorized()
    {
        // Arrange
        string itemId = "651c1b04ae02a3135d6439fc"; // Rvalid id

        // Act
        var response = await HttpClient.DeleteAsync($"{ResourceUrl}/{itemId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion
}

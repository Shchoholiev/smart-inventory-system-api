using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Application.Paging;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class ShelvesControllerTests : TestsBase
{
    public ShelvesControllerTests(TestingFactory<Program> factory)
        : base(factory, "shelves")
    { }

    #region GetShelvesPageAsync

    [Fact]
    public async Task GetShelvesPageAsync_GroupWithShelves_ReturnsShelvesPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        int page = 1;
        int size = 10;
        string groupId = "652c3b89ae02a3135d6429fc"; // group with shelves

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");
        var shelvesList = await response.Content.ReadFromJsonAsync<PagedList<ShelfDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(shelvesList);
        Assert.True(shelvesList.Items.Count() > 0);
    }

    [Fact]
    public async Task GetShelvesPageAsync_GroupWithoutShelves_ReturnsEmptyShelvesPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        int page = 1;
        int size = 10;
        string groupId = "662c3b89ae02a3135d6429fc"; // group without shelves

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");
        var shelvesList = await response.Content.ReadFromJsonAsync<PagedList<ShelfDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(shelvesList);
        Assert.Empty(shelvesList.Items);
    }

    [Fact]
    public async Task GetShelvesPageAsync_UnauthorizedUser_ReturnsUnathorized()
    {
        // Arrange
        int page = 1;
        int size = 10;
        string groupId = "652c3b89ae02a3135d6429fc"; // group with shelves

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetShelvesPageAsync_GroupWithShelvesAndSearch_ReturnsShelvesPagedList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        var page = 1;
        var size = 10;
        var groupId = "652c3b89ae02a3135d6429fc"; // group with shelves
        var search = "Shelf 1";

        // Act
        var response = await HttpClient.GetAsync(
            $"{ResourceUrl}?page={page}&size={size}&groupId={groupId}&search={search}");
        var shelvesList = await response.Content.ReadFromJsonAsync<PagedList<ShelfDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(shelvesList);
        Assert.NotEmpty(shelvesList.Items);
        Assert.True(shelvesList.Items.All(s => s.Name.Contains(search)));
    }

    #endregion

    #region GetShelfAsync

    [Fact]
    public async Task GetShelfAsync_ExistingShelfId_ReturnsShelfDto()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string shelfId = "651c1b09ae02a3135d6439fc";  // valid shelf Id

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{shelfId}");
        var shelfDto = await response.Content.ReadFromJsonAsync<ShelfDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(shelfDto);
        Assert.Equal(shelfId, shelfDto.Id);
    }

    [Fact]
    public async Task GetShelfAsync_NonExistingShelfId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string invalidShelfId = "651c5b09ae02a3135d6439fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{invalidShelfId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetShelfAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        string shelfId = "651c1b09ae02a3135d6439fc";  // valid shelf Id

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{shelfId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region GetShelfItemsAsync

    [Fact]
    public async Task GetShelfItemsAsync_ValidShelfIdWithItems_ReturnsItemList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string shelfId = "651c1b09ae02a3135d6439fc"; // shelf with items

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{shelfId}/items");
        var itemsList = await response.Content.ReadFromJsonAsync<List<ItemDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemsList);
        Assert.True(itemsList.Count > 0);
    }

    [Fact]
    public async Task GetShelfItemsAsync_ValidShelfIdWithoutItems_ReturnsEmptyItemList()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string shelfId = "651c3b09ae02a3135d6439fc"; // shelf without items

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{shelfId}/items");
        var itemsList = await response.Content.ReadFromJsonAsync<List<ItemDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(itemsList);
        Assert.Empty(itemsList);
    }

    [Fact]
    public async Task GetShelfItemsAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        string shelfId = "651c1b09ae02a3135d6439fc"; // shelf with items

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{shelfId}/items");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region AddItemAsync

    [Fact]
    public async Task AddItemAsync_ValidShelfIdAndItemDto_ReturnsCreatedItem()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string shelfId = "651c1b09ae02a3135d6439fc"; // valid shelf ID
        var newItem = new ItemCreateDto
        {
            Name = "New Item",
            Description = "This is a new item"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{shelfId}/items", newItem);
        var responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine(responseContent);
        var createdItem = await response.Content.ReadFromJsonAsync<ItemDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(createdItem);
        Assert.Equal(newItem.Name, createdItem.Name);
        Assert.Equal(newItem.Description, createdItem.Description);
    }

    [Fact]
    public async Task AddItemAsync_InvalidShelfId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string invalidShelfId = "651c1b04ae02a3135d6139fc";
        var newItem = new ItemCreateDto
        {
            Name = "New Item",
            Description = "This is a new item"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{invalidShelfId}/items", newItem);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AddItemAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        string shelfId = "651c1b09ae02a3135d6439fc"; // valid shelf ID
        var newItem = new ItemCreateDto
        {
            Name = "New Item",
            Description = "This is a new item"
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}/{shelfId}/items", newItem);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region UpdateShelfAsync

    [Fact]
    public async Task UpdateShelfAsync_ValidShelfIdAndDto_ReturnsUpdatedShelf()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        string shelfId = "651c4b89ae02a3135d6439fc"; // valid shelf Id
        var updatedShelfDto = new ShelfUpdateDto
        {
            Name = "Updated Shelf"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{shelfId}", updatedShelfDto);
        var content = await response.Content.ReadAsStringAsync();
        var updatedShelf = await response.Content.ReadFromJsonAsync<ShelfDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedShelf);
        Assert.Equal(updatedShelfDto.Name, updatedShelf.Name);
    }

    [Fact]
    public async Task UpdateShelfAsync_InvalidShelfId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        string invalidShelfId = "651c1b09ae13a3135d6439fc";
        var updatedShelfDto = new ShelfDto
        {
            Name = "Updated Shelf"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{invalidShelfId}", updatedShelfDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfAsync_NonOwnerUser_ReturnsForbidden()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string invalidShelfId = "651c1b09ae13a3135d6439fc";
        var updatedShelfDto = new ShelfUpdateDto
        {
            Name = "Updated Shelf"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{invalidShelfId}", updatedShelfDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        string shelfId = "651c1b09ae02a3135d6439fc"; // valid shelf Id
        var updatedShelfDto = new ShelfUpdateDto
        {
            Name = "Updated Shelf"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{shelfId}", updatedShelfDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region UpdateShelfStatusAsync 

    // TODO: Add happy path tests after IoT device is implemented

    [Fact]
    public async Task UpdateShelfStatusAsync_InvalidShelfId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string invalidShelfId = "651c4b11ae02a3135d6439fc";
        var statusChangeDto = new ShelfStatusChangeDto
        {
            ItemId = "651c1b01ae02a3135d6439fc", // valid Item Id
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{invalidShelfId}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfStatusAsync_InvalidItemId_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("group@gmail.com", "Yuiop12345");
        string validShelfId = "651c1b09ae02a3135d6439fc"; // valid shelf ID
        var statusChangeDto = new ShelfStatusChangeDto
        {
            ItemId = "651c1b11ae02a3135d6439fc", // invalid Item ID
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{validShelfId}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfStatusAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        string validShelfId = "651c1b09ae02a3135d6439fc"; // valid shelf ID
        var statusChangeDto = new ShelfStatusChangeDto
        {
            ItemId = "651c1b01ae02a3135d6439fc", // valid Item ID
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{validShelfId}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

}

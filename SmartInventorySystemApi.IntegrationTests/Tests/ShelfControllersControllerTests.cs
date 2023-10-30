using System.Net;
using System.Net.Http.Json;
using Newtonsoft.Json;
using SmartInventorySystemApi.Application.Models;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class ShelfControllersControllerTests : TestsBase
{
    public ShelfControllersControllerTests(TestingFactory<Program> factory)
        : base(factory, "shelf-controllers")
    { }

    #region UpdateShelfStatusAsync Tests

    [Fact]
    public async Task UpdateShelfStatusAsync_ValidRequest_UpdatesShelfStatus()
    {
        // Arrange
        var deviceGuid = "7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f";
        var shelfPosition = 1;
        var statusChangeDto = new ShelfControllerStatusChangeDto
        {
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfStatusAsync_NonExistingDeviceGuid_ReturnsNotFound()
    {
        // Arrange
        var deviceGuid = "77339206-8bdc-4841-9066-b637b8a9c6f3";
        var shelfPosition = 1;
        var statusChangeDto = new ShelfControllerStatusChangeDto
        {
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateShelfStatusAsync_MissingShelf_ReturnsNotFound()
    {
        // Arrange
        var deviceGuid = "7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f";
        var shelfPosition = 99; // Assume there's no shelf at this position
        var statusChangeDto = new ShelfControllerStatusChangeDto
        {
            IsLitUp = true
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/status", statusChangeDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

    #region HandleMovementAsync Tests

    [Fact]
    public async Task HandleMovementAsync_ValidRequest_HandlesMovement()
    {
        // Arrange
        var deviceGuid = "7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f";
        var shelfPosition = 1;

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/movements", null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        // Additional assertions can be made here to validate the behavior
    }

    [Fact]
    public async Task HandleMovementAsync_NonExistingDeviceGuid_ReturnsNotFound()
    {
        // Arrange
        var deviceGuid = "77339206-8bdc-4841-9066-b637b8a9c6f3";
        var shelfPosition = 1;

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/movements", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task HandleMovementAsync_MissingShelf_ReturnsNotFound()
    {
        // Arrange
        var deviceGuid = "7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f";
        var shelfPosition = 99; // Assume there's no shelf at this position

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/movements", null);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    // TODO: Uncomment when IoT device is implemented
    // [Fact]
    // public async Task HandleMovementAsync_ShelfLit_HandlesLightControl()
    // {
    //     // Arrange
    //     var deviceGuid = "7a78a8b2-6cf6-427d-8ed2-a5e117d8fd3f";
    //     var shelfPosition = 4;  // Assume this shelf is lit

    //     // Act
    //     var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/shelf/{shelfPosition}/movements", null);

    //     // Assert
    //     Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    // }

    #endregion
}

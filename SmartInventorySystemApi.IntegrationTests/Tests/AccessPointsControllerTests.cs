using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using Microsoft.AspNetCore.Http;
using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class AccessPointsControllerTests : TestsBase
{
    public AccessPointsControllerTests(TestingFactory<Program> factory)
        : base(factory, "access-points")
    { }

    #region GetScansHistoryAsync

    [Fact]
    public async Task GetScansHistoryAsync_ValidInput_ReturnsScanHistoryDtoList()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var deviceId = "753c3b89ae02a3135d6139fc"; // valid DeviceId
        var page = 1;
        var size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}/scans-history?page={page}&size={size}");
        var scanHistoryDtoList = await response.Content.ReadFromJsonAsync<List<ScanHistoryDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(scanHistoryDtoList);
        Assert.NotEmpty(scanHistoryDtoList);
    }

    [Fact]
    public async Task GetScansHistoryAsync_UnauthorizedUser_ReturnsUnathorized()
    {
        // Arrange
        var deviceId = "753c3b89ae02a3135d6139fc"; // valid DeviceId
        var page = 1;
        var size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}/scans-history?page={page}&size={size}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetScansHistoryAsync_EmptyPage_ReturnsEmptyList()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var deviceId = "753c3b89ae02a3135d6139fc";
        var page = 999; // Assuming there aren't this many pages of data
        var size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}/scans-history?page={page}&size={size}");
        var scanHistoryDtoList = await response.Content.ReadFromJsonAsync<List<ScanHistoryDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(scanHistoryDtoList);
        Assert.Empty(scanHistoryDtoList);
    }

    #endregion

    #region FindItemByImageAsync

    [Fact]
    public async Task FindItemByImageAsync_ItemImage_ReturnsOk()
    {
        // Arrange
        var deviceGuid = "4d09b6ae-7675-4603-b632-9e834de6957f"; // replace with a valid device GUID

        var projectDir = Environment.CurrentDirectory;
        var imagePath = Path.Combine(projectDir, "Media", "charger-image.png");
        
        byte[] imageData = File.ReadAllBytes(imagePath);
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        var form = new MultipartFormDataContent
        {
            { imageContent, "image", "charger-image.png" }
        };

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/items/identify-by-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task FindItemByImageAsync_QRCodeImage_ReturnsOk()
    {
        // Arrange
        var deviceGuid = "4d09b6ae-7675-4603-b632-9e834de6957f";

        var projectDir = Environment.CurrentDirectory;
        var imagePath = Path.Combine(projectDir, "Media", "charger-with-qrcode.png");
        
        byte[] imageData = File.ReadAllBytes(imagePath);
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        var form = new MultipartFormDataContent
        {
            { imageContent, "image", "charger-with-qrcode.png" }
        };

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/items/identify-by-image", form);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task FindItemByImageAsync_NoImage_ReturnsBadRequest()
    {
        // Arrange
        var deviceGuid = "4d09b6ae-7675-4603-b632-9e834de6957f"; // replace with a valid device GUID
        var form = new MultipartFormDataContent();

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/items/identify-by-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task FindItemByImageAsync_NonExistingDeviceGuid_ReturnsNotFound()
    {
        // Arrange
        var deviceGuid = "4d09b6ae-7685-4603-b632-9e834de6957f"; // replace with a valid device GUID

        var projectDir = Environment.CurrentDirectory;
        var imagePath = Path.Combine(projectDir, "Media", "charger-with-qrcode.png");
        
        byte[] imageData = File.ReadAllBytes(imagePath);
        var imageContent = new ByteArrayContent(imageData);
        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/png");
        var form = new MultipartFormDataContent
        {
            { imageContent, "image", "charger-with-qrcode.png" }
        };

        // Act
        var response = await HttpClient.PostAsync($"{ResourceUrl}/{deviceGuid}/items/identify-by-image", form);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    #endregion

}

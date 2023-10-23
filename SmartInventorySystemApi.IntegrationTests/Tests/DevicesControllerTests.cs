using System.Net;
using System.Net.Http.Json;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.AdminDto;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.IntegrationTests.Tests;

public class DevicesControllerTests : TestsBase
{
    public DevicesControllerTests(TestingFactory<Program> factory)
        : base(factory, "devices")
    { }

    #region GetDeviceAsync

    [Fact]
    public async Task CreateDeviceAsync_ValidInput_ReturnsDeviceAdminDto()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var deviceCreateDto = new DeviceCreateDto
        {
            Name = "Integration Test Shelf Rack",
            Type = DeviceType.Rack4ShelfController
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", deviceCreateDto);
        var deviceAdminDto = await response.Content.ReadFromJsonAsync<DeviceAdminDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(deviceAdminDto);
        Assert.Equal("Integration Test Shelf Rack", deviceAdminDto.Name);
    }

    [Fact]
    public async Task CreateDeviceAsync_MissingDeviceType_ReturnsBadRequest()
    {
        // Arrange
        await LoginAsync("admin@gmail.com", "Yuiop12345");
        var deviceCreateDto = new DeviceCreateDto
        {
            // No device type should cause a bad request
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", deviceCreateDto);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateDeviceAsync_NonAdminUser_ReturnsForbidden()
    {
        // Arrange
        await LoginAsync("test@gmail.com", "Yuiop12345");
        var deviceCreateDto = new DeviceCreateDto
        {
            // No device type should cause a bad request
        };

        // Act
        var response = await HttpClient.PostAsJsonAsync($"{ResourceUrl}", deviceCreateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    #endregion

    #region GetDeviceAsync

    [Fact]
    public async Task GetDeviceAsync_DeviceExists_ReturnsDeviceDto()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "651c3b89ae02a3135d6439fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}");
        var deviceDto = await response.Content.ReadFromJsonAsync<DeviceDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(deviceDto);
        Assert.Equal(deviceId, deviceDto.Id);
    }

    [Fact]
    public async Task GetDeviceAsync_DeviceDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "651c3b89ae02a3135d6435fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetDeviceAsync_UnauthorizedUser_ReturnsUnathorized()
    {
        // Arrange
        var deviceId = "651c3b89ae02a3135d6439fc";

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}/{deviceId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region GetDevicesPageAsync

    [Fact]
    public async Task GetDevicesPageAsync_DevicesExistForGroup_ReturnsDeviceList()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "652c3b89ae02a3135d6429fc";
        int page = 1;
        int size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");
        var content = await response.Content.ReadAsStringAsync();
        var deviceDtos = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(deviceDtos);
        Assert.True(deviceDtos.Count > 0);
    }

    [Fact]
    public async Task GetDevicesPageAsync_NoDevicesExistForGroup_ReturnsEmptyList()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var groupId = "611c3b89ae02a3135d6429fc";
        int page = 1;
        int size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");
        var deviceDtos = await response.Content.ReadFromJsonAsync<List<DeviceDto>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(deviceDtos);
        Assert.Empty(deviceDtos);
    }

    [Fact]
    public async Task GetDevicesPageAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var groupId = "652c3b89ae02a3135d6429fc";
        int page = 1;
        int size = 10;

        // Act
        var response = await HttpClient.GetAsync($"{ResourceUrl}?page={page}&size={size}&groupId={groupId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region UpdateDeviceAsync

    [Fact]
    public async Task UpdateDeviceAsync_DeviceExists_UpdatesSuccessfully()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "653c3b89ae02a3135d6439fc";
        var updateDto = new DeviceUpdateDto 
        { 
            Name = "Updated Device"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{deviceId}", updateDto);
        var updatedDevice = await response.Content.ReadFromJsonAsync<DeviceDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedDevice);
    }

    [Fact]
    public async Task UpdateDeviceAsync_DeviceDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "691c3b89ae02a3135d6439fc";
        var updateDto = new DeviceUpdateDto 
        { 
             Name = "Updated Device"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{deviceId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDeviceAsync_UnauthorizedUser_ReturnsUnauthorized()
    {
        // Arrange
        var deviceId = "653c3b89ae02a3135d6439fc";
        var updateDto = new DeviceUpdateDto 
        { 
             Name = "Updated Device"
        };

        // Act
        var response = await HttpClient.PutAsJsonAsync($"{ResourceUrl}/{deviceId}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    #endregion

    #region UpdateDeviceStatusAsync

    [Fact]
    public async Task UpdateDeviceStatusAsync_DeviceExists_UpdatesSuccessfully()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "653c3b89ae02a3135d6439fc";
        var statusDto = new DeviceStatusChangeDto 
        { 
            IsActive = true, 
            GroupId = "652c3b89ae02a3135d6429fc" 
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceId}/status", statusDto);
        var updatedDevice = await response.Content.ReadFromJsonAsync<DeviceDto>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(updatedDevice);
        Assert.True(updatedDevice.IsActive);
    }

    [Fact]
    public async Task UpdateDeviceStatusAsync_DeviceDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "611c3b89ae02a3135d6439fc";
        var statusDto = new DeviceStatusChangeDto 
        { 
            IsActive = true, 
            GroupId = "652c3b89ae02a3135d6429fc" 
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceId}/status", statusDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateDeviceStatusAsync_AttemptDeactivation_ReturnsNotImplemented()
    {
        // Arrange
        await LoginAsync("owner@gmail.com", "Yuiop12345");
        var deviceId = "653c3b89ae02a3135d6439fc";
        var statusDto = new DeviceStatusChangeDto 
        { 
            IsActive = false
        };

        // Act
        var response = await HttpClient.PatchAsJsonAsync($"{ResourceUrl}/{deviceId}/status", statusDto);

        // Assert
        Assert.Equal(HttpStatusCode.NotImplemented, response.StatusCode);
    }

    #endregion
}

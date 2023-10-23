using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.AdminDto;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.UpdateDto;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("devices")]
public class DevicesController : ApiController
{
    private readonly IDevicesService _devicesService;

    public DevicesController(
        IDevicesService devicesService)
    {
        _devicesService = devicesService;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<DeviceAdminDto> CreateDeviceAsync(DeviceCreateDto deviceCreateDto, CancellationToken cancellationToken)
    {
        return await _devicesService.CreateDeviceAsync(deviceCreateDto, cancellationToken);
    }
    
    [HttpGet("{deviceId}")]
    [Authorize(Roles = "Owner")]
    public async Task<DeviceDto> GetDeviceAsync(string deviceId, CancellationToken cancellationToken)
    {
        return await _devicesService.GetDeviceAsync(deviceId, cancellationToken);
    }

    [HttpGet]
    [Authorize(Roles = "Owner")]
    public async Task<List<DeviceDto>> GetDevicesPageAsync(int page, int size, string groupId, CancellationToken cancellationToken)
    {
        return await _devicesService.GetDevicesPageAsync(page, size, groupId, cancellationToken);
    }

    [HttpPatch("{deviceId}/status")]
    [Authorize(Roles = "Owner")]
    public async Task<DeviceDto> UpdateDeviceStatusAsync(string deviceId, DeviceStatusChangeDto deviceDto, CancellationToken cancellationToken)
    {
        return await _devicesService.UpdateDeviceStatusAsync(deviceId, deviceDto, cancellationToken);
    }

    [HttpPut("{deviceId}")]
    [Authorize(Roles = "Owner")]
    public async Task<DeviceDto> UpdateDeviceAsync(string deviceId, DeviceUpdateDto deviceUpdateDto, CancellationToken cancellationToken)
    {
        return await _devicesService.UpdateDeviceAsync(deviceId, deviceUpdateDto, cancellationToken);
    }
}
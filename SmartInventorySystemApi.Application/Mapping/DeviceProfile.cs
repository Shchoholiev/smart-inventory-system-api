using AutoMapper;
using SmartInventorySystemApi.Application.Models;
using SmartInventorySystemApi.Application.Models.AdminDto;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Mapping;

public class DeviceProfile : Profile
{
    public DeviceProfile()
    {
        CreateMap<Device, DeviceDto>();
        CreateMap<Device, DeviceAdminDto>();
        CreateMap<DeviceDto, Device>();
        CreateMap<DeviceCreateDto, Device>();
        CreateMap<DeviceUpdateDto, Device>();
        CreateMap<DeviceUpdateDto, Device>();
    }
}

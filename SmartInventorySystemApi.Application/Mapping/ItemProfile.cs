using AutoMapper;
using SmartInventorySystemApi.Application.Models.CreateDto;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Mapping;

public class ItemProfile : Profile
{
    public ItemProfile()
    {
        CreateMap<Item, ItemDto>();
        CreateMap<ItemDto, Item>();
        CreateMap<ItemCreateDto, Item>();
    }
}
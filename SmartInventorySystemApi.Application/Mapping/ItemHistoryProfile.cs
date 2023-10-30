using AutoMapper;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Mapping;

public class ItemHistoryProfile : Profile
{
    public ItemHistoryProfile()
    {
        CreateMap<ItemHistory, ItemHistoryDto>();
    }
}

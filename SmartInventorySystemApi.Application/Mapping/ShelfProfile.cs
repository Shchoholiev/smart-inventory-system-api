using AutoMapper;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Application.Models.Statistics;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Mapping;

public class ShelfProfile : Profile
{
    public ShelfProfile()
    {
        CreateMap<Shelf, ShelfDto>();
        CreateMap<ShelfDto, Shelf>();
        CreateMap<ShelfUpdateDto, Shelf>();

        CreateMap<ShelfLoadLookup, ShelfLoad>();
    }
}
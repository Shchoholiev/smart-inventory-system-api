using AutoMapper;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Domain.Entities;

namespace SmartInventorySystemApi.Application.Mapping;

public class ScanHistoryProfile : Profile
{
    public ScanHistoryProfile()
    {
        CreateMap<ScanHistory, ScanHistoryDto>();
    }
}

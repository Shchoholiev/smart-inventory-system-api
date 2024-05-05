using AutoMapper;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.Lookup;
using SmartInventorySystemApi.Application.Models.Statistics;
using SmartInventorySystemApi.Application.Models.UpdateDto;
using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<UserUpdateDto, User>();

        CreateMap<UserActivityLookup, UserActivity>();
        CreateMap<UserDebtLookup, UserDebt>();
    }
}

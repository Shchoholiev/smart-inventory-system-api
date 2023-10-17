using AutoMapper;
using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.Mapping;

public class UserProfile: Profile
{
    public UserProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
    }
}

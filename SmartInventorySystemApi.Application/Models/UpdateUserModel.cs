using SmartInventorySystemApi.Application.Models.Dto;
using SmartInventorySystemApi.Application.Models.Identity;

namespace SmartInventorySystemApi.Application.Models;

public class UpdateUserModel
{
    public TokensModel Tokens { get; set; }

    public UserDto User { get; set; }
}

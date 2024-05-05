using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.Models.Statistics;

public class UserActivity
{
    public UserDto User { get; set; }

    public int ActionsCount { get; set; }
}

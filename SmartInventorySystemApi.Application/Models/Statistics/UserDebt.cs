using SmartInventorySystemApi.Application.Models.Dto;

namespace SmartInventorySystemApi.Application.Models.Statistics;

public class UserDebt
{
    public UserDto User { get; set; }

    public int ItemsTakenCount { get; set; }
}

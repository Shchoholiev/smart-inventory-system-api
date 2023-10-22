using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Application.Models.CreateDto;

public class DeviceCreateDto
{
    public string? Name { get; set; }

    public DeviceType Type { get; set; }

    public string? GroupId { get; set; }
}

using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Application.Models.Dto;

public class DeviceDto
{
    public string Id { get; set; }

    public string? Name { get; set; }

    public DeviceType Type { get; set; }

    public Guid Guid { get; set; }

    public string? GroupId { get; set; }

    public bool IsActive { get; set; }
}

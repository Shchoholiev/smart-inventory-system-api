using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Domain.Entities;

public class Group : EntityBase
{
    public string Name { get; set; }

    public string? Description { get; set; }
}

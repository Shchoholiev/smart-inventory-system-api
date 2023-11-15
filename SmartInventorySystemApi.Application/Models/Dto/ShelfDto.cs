using MongoDB.Bson;

namespace SmartInventorySystemApi.Application.Models.Dto;

public class ShelfDto
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public bool IsLitUp { get; set; }

    public string GroupId { get; set; }

    public string DeviceId { get; set; }
}

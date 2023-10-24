using MongoDB.Bson;

namespace SmartInventorySystemApi.Application.Models.Dto;

public class ShelfDto
{
    public string Name { get; set; }

    public bool IsLitUp { get; set; }

    public ObjectId GroupId { get; set; }

    public ObjectId DeviceId { get; set; }
}

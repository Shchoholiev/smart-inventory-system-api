using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Domain.Entities;

public class Shelf : EntityBase
{
    public string Name { get; set; }

    public bool IsLitUp { get; set; }

    public int PositionInRack { get; set; }

    public ObjectId GroupId { get; set; }

    public ObjectId DeviceId { get; set; }
}

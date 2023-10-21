using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SmartInventorySystemApi.Domain.Common;
using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Domain.Entities;

public class Device : EntityBase
{
    public string? Name { get; set; }

    [BsonRepresentation(BsonType.String)]
    public DeviceType Type { get; set; }

    public ObjectId GroupId { get; set; }
}

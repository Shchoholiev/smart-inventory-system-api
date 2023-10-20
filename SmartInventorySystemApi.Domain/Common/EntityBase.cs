using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SmartInventorySystemApi.Domain.Common;

public class EntityBase
{
    [BsonId]
    public ObjectId Id { get; set; }

    public ObjectId CreatedById { get; set; }

    public DateTime CreatedDateUtc { get; set; }

    public bool IsDeleted { get; set; }

    public ObjectId? LastModifiedById { get; set; }

    public DateTime? LastModifiedDateUtc { get; set; }
}

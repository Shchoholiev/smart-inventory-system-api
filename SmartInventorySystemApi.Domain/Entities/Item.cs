using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Domain.Entities;

// TODO: add image
public class Item : EntityBase
{
    public string Name { get; set; }

    public string? Description { get; set; }
    
    public bool IsTaken { get; set; }

    public ObjectId ShelfId { get; set; }

    public ObjectId GroupId { get; set; }
}

using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;
using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Domain.Entities;

public class ItemHistory : EntityBase
{
    public ItemHistoryType Type { get; set; }

    public ObjectId ItemId { get; set; }
    
    public bool IsTaken { get; set; }

    public string? Comment { get; set; }
}
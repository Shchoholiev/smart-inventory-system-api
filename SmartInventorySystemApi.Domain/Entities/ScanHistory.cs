using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;
using SmartInventorySystemApi.Domain.Enums;

namespace SmartInventorySystemApi.Domain.Entities;

public class ScanHistory : EntityBase
{
    public ScanType ScanType { get; set; }

    public string Result { get; set; }

    public ObjectId DeviceId { get; set; }
}

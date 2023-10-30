using MongoDB.Bson;
using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Domain.Entities.Identity;

public class User : EntityBase
{
    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public ObjectId? GroupId { get; set; }

    public List<Role> Roles { get; set; }
}

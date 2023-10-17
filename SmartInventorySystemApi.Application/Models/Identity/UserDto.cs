using MongoDB.Bson;

namespace SmartInventorySystemApi.Application.Models.Identity;

public class UserDto
{
    public string Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public ObjectId? GroupId { get; set; }

    public List<RoleDto> Roles { get; set; }
}

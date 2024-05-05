using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.Models.Lookup;

public class UserActivityLookup
{
    public User User { get; set; }

    public int ActionsCount { get; set; }
}

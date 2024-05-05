using SmartInventorySystemApi.Domain.Entities.Identity;

namespace SmartInventorySystemApi.Application.Models.Lookup;

public class UserDebtLookup
{
    public User User { get; set; }

    public int ItemsTakenCount { get; set; }
}

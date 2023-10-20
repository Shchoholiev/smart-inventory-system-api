using SmartInventorySystemApi.Domain.Common;

namespace SmartInventorySystemApi.Domain.Entities.Identity;

public class RefreshToken : EntityBase
{
    public string Token { get; set; }

    public DateTime ExpiryDateUTC { get; set; }
}

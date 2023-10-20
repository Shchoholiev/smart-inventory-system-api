using System.Security.Claims;

namespace SmartInventorySystemApi.Application.IServices.Identity;

public interface ITokensService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);

    string GenerateRefreshToken();

    ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
}

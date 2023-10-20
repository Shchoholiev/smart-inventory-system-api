using Microsoft.AspNetCore.Mvc;
using SmartInventorySystemApi.Application.IServices.Identity;
using SmartInventorySystemApi.Application.Models.Identity;

namespace SmartInventorySystemApi.Api.Controllers;

[Route("tokens")]
public class TokensController : ApiController
{
    private readonly IUserManager _userManager;

    public TokensController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokensModel>> RefreshAccessTokenAsync([FromBody] TokensModel tokensModel, CancellationToken cancellationToken)
    {
        var refreshedTokens = await _userManager.RefreshAccessTokenAsync(tokensModel, cancellationToken);
        return Ok(refreshedTokens);
    }
}

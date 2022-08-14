using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.DTO;

namespace Template.AuthenticationAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[EnableCors("CorsPolicy")]
public class AccountController : Controller
{
    private readonly IAntiForgeryCookieService _antiforgery;
    private readonly ITokenFactoryService _tokenFactoryService;
    private readonly ITokenStoreService _tokenStoreService;
    private readonly IUsersService _usersService;

    public AccountController(
        IUsersService usersService,
        ITokenStoreService tokenStoreService,
        ITokenFactoryService tokenFactoryService,
        IAntiForgeryCookieService antiforgery)
    {
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
        _tokenStoreService = tokenStoreService ?? throw new ArgumentNullException(nameof(tokenStoreService));
        _antiforgery = antiforgery ?? throw new ArgumentNullException(nameof(antiforgery));
        _tokenFactoryService = tokenFactoryService ?? throw new ArgumentNullException(nameof(tokenFactoryService));
    }

    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUser)
    {
        if (loginUser == null)
        {
            return BadRequest("user is not set.");
        }

        var user = await _usersService.FindUserAsync(loginUser.UserName, loginUser.Password);
        if (user?.IsActive != true)
        {
            return Unauthorized();
        }

        var result = await _tokenFactoryService.CreateJwtTokensAsync(user);
        await _tokenStoreService.AddUserTokenAsync(user, result.RefreshTokenSerial, result.AccessToken, null);

        _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

        return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
    }

    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDTO model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        var refreshTokenValue = model.RefreshToken;
        if (string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return BadRequest("refreshToken is not set.");
        }

        var token = await _tokenStoreService.FindTokenAsync(refreshTokenValue);
        if (token == null)
        {
            return Unauthorized("This is not our token!");
        }

        var user = _usersService.FindUserAsync(token.UserId);
        
        var result = await _tokenFactoryService.CreateJwtTokensAsync(user.Result);
        await _tokenStoreService.AddUserTokenAsync(user.Result, result.RefreshTokenSerial, result.AccessToken,
            _tokenFactoryService.GetRefreshTokenSerial(refreshTokenValue));

        _antiforgery.RegenerateAntiForgeryCookies(result.Claims);

        return Ok(new { access_token = result.AccessToken, refresh_token = result.RefreshToken });
    }

    [AllowAnonymous]
    [HttpGet("[action]")]
    public async Task<bool> Logout(string refreshToken)
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userIdValue = claimsIdentity?.FindFirst(ClaimTypes.UserData)?.Value;

        // The Jwt implementation does not support "revoke OAuth token" (logout) by design.
        // Delete the user's tokens from the database (revoke its bearer token)
        await _tokenStoreService.RevokeUserBearerTokensAsync(userIdValue, refreshToken);

        _antiforgery.DeleteAntiForgeryCookies();

        return true;
    }

    [HttpGet("[action]")]
    public bool IsAuthenticated()
    {
        return User.Identity?.IsAuthenticated ?? false;
    }

    [HttpGet("[action]")]
    public IActionResult GetUserInfo()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        return Json(new { Username = claimsIdentity?.Name });
    }
    
    //[ValidateAntiForgeryToken] <---- Enable this for validating AntiForgery cookie!
    [IgnoreAntiforgeryToken]
    [HttpPost("[action]")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
    {
        if (model == null)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _usersService.GetCurrentUserAsync();
        if (user == null)
        {
            return BadRequest("NotFound");
        }

        var (succeeded, error) = await _usersService.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
        if (succeeded)
        {
            return Ok();
        }

        return BadRequest(error);
    }
}
using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Template.AuthenticationAPI.Interfaces;
using Template.Data.Infrastructure.Common;

namespace Template.AuthenticationAPI.Controllers;

[Route("api/[controller]")]
[EnableCors("CorsPolicy")] 
[Authorize(Policy = CustomRoles.Admin)]
public class TestAdminController : Controller
{
    private readonly IUsersService _usersService;

    public TestAdminController(IUsersService usersService)
    {
        _usersService = usersService ?? throw new ArgumentNullException(nameof(usersService));
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var claimsIdentity = User.Identity as ClaimsIdentity;
        var userDataClaim = claimsIdentity?.FindFirst(ClaimTypes.UserData);
        var userId = userDataClaim?.Value;

        return Ok(new
        {
            Id = 1,
            Title = "Hi from Admin Controller! [Authorize(Policy = CustomRoles.Admin)]",
            Username = User.Identity?.Name,
            UserData = userId,
            TokenSerialNumber =
                await _usersService.GetSerialNumberAsync(int.Parse(userId ?? "0", NumberStyles.Number,
                    CultureInfo.InvariantCulture)),
            Roles = claimsIdentity?.Claims.Where(x => string.Equals(x.Type, ClaimTypes.Role, StringComparison.Ordinal))
                .Select(x => x.Value).ToList()
        });
    }
}
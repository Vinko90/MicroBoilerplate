using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace Template.AuthenticationAPI.Controllers;

[Route("api/[controller]")]
[EnableCors("CorsPolicy")]
[Authorize]
public class TestAuthorizeController : Controller
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            Id = 1,
            Title = "Hi from Authorize Controller! [Authorize]",
            Username = User.Identity?.Name
        });
    }
}
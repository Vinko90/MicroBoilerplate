using Microsoft.AspNetCore.Mvc;

namespace Template.ServiceOneAPI.Controllers;

[ApiController]
[Route("service-one/[controller]")]
public class OneController : ControllerBase
{
    private static readonly string[] OneList = {
        "Blue", "Yellow", "Red"
    };
    
    //GET service-one/
    [HttpGet("")]
    public Task<IActionResult> Get()
    {
        return Task.FromResult<IActionResult>(Ok(OneList));
    }
}
using Microsoft.AspNetCore.Mvc;

namespace Template.ServiceTwoAPI.Controllers;

[ApiController]
[Route("service-two/[controller]")]
public class TwoController : ControllerBase
{
    private static readonly string[] TwoList = {
        "C", "C++", "C#"
    };
    
    //GET service-two/
    [HttpGet("")]
    public Task<IActionResult> Get()
    {
        return Task.FromResult<IActionResult>(Ok(TwoList));
    }
}
using Microsoft.AspNetCore.Mvc;
using Template.Data.Infrastructure.Managers;

namespace Template.TestWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IRolesService _rolesService;

    public TestController(ILogger<TestController> logger, IRolesService rolesService)
    {
        _logger = logger;
        _rolesService = rolesService;
    }

    [HttpGet("getrolesbyuserid")]
    public Task<IActionResult> GetRolesByUserId(int id)
    {
        var items = _rolesService.FindUserRolesAsync(id);
        return Task.FromResult<IActionResult>(Ok(items));
    }

    [HttpGet("getrolesbyuserid/{userId:int}")]
    public Task<IActionResult> IsUserInRole(int userId, string roleName)
    {
        return Task.FromResult<IActionResult>(Ok(_rolesService.IsUserInRoleAsync(userId, roleName)));
    }
    
    [HttpGet("findallusersinrole")]
    public Task<IActionResult> FindAllUsersInRole(string roleName)
    {
        return Task.FromResult<IActionResult>(Ok(_rolesService.FindUsersInRoleAsync(roleName)));
    }
    
    //[HttpGet("finduser")]
    //public Task<IActionResult> FindUser(string username, string password)
    //{
     //   return Task.FromResult<IActionResult>(Ok(_rolesService.FindUser(username, password)));
   //}
}
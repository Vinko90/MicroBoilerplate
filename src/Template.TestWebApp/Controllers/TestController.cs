using Microsoft.AspNetCore.Mvc;
using Template.TestWebApp.Manager;

namespace Template.TestWebApp.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly ILogger<TestController> _logger;
    private readonly IDataManager _manager;

    public TestController(ILogger<TestController> logger, IDataManager manager)
    {
        _logger = logger;
        _manager = manager;
    }

    [HttpGet("getbyid")]
    public async Task<IActionResult> GetById(int id, CancellationToken cancellationToken)
    {
        //var weatherForecasts = await _factory.WeatherForecasts.GetByIdAsync(id, cancellationToken);
        //return Ok(weatherForecasts);
        var items = _manager.GetById(1);
        return Ok(items);
    }
    
    [HttpGet("getall")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = _manager.GetById(1);
        //var weatherForecasts = await _factory.WeatherForecasts.GetForecastsAsync(cancellationToken);
        //return Ok(weatherForecasts);
        return Ok(items);
    }
    
    [HttpPost("add")]
    public async Task<IActionResult> Add(CancellationToken cancellationToken)
    {
        return Ok();
    }
    
}
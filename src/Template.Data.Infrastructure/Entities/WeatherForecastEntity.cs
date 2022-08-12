using RepoDb.Attributes;

namespace Template.Data.Infrastructure.Entities;

[Map("weatherforecasts")]
public class WeatherForecastEntity
{
    [Map("id")]
    public int Id { get; set; }
    
    [Map("date")]
    public DateTime Date { get; set; }

    [Map("temperature_c")]
    public int TemperatureC { get; set; }

    [Map("summary")]
    public string Summary { get; set; }
}
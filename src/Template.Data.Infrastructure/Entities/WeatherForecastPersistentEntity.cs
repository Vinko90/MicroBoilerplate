using Dapper.Contrib.Extensions;

namespace Template.Data.Infrastructure.Entities;

[Table("WeatherForecasts")]
public class WeatherForecastPersistentEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public string Summary { get; set; }
}
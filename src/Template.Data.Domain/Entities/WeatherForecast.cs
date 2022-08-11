using Template.Data.Domain.Abstractions;

namespace Template.Data.Domain.Entities;

public class WeatherForecast : BaseEntity<int>
{
    public DateTime Date { get; private set; }

    public int TemperatureC { get; private set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    
        
    public WeatherForecast()
    {
    }
        
    public WeatherForecast(int id) : base(id)
    {
    }

    public WeatherForecast SetDate(DateTime date)
    {
        Date = date;
        return this;
    }
        
    public WeatherForecast SetCelciusTemperature(int temperatureC)
    {
        TemperatureC = temperatureC;
        return this;
    }
}
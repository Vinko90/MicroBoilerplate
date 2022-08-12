using Microsoft.Extensions.Options;
using Template.Data.Infrastructure.Entities;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Repositories.Integration;

public class WeatherForecastRepository : 
    CommonRepository<WeatherForecastEntity>, IWeatherForecastRepository
{

    public WeatherForecastRepository(IOptions<DbSettings> options)
        : base(options)
    {
    }
}
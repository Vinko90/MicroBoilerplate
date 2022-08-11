using Template.Data.Domain.Entities;
using Template.Data.Domain.Interfaces;

namespace Template.Data.Domain.Repositories;

public interface IWeatherForecastsRepository : IRepository<WeatherForecast, int>
{
    Task<List<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken);
}
using Template.Data.Domain.Repositories;

namespace Template.Data.Domain.Interfaces;

public interface IDataFactory : IUnitOfWork
{
    IWeatherForecastsRepository WeatherForecasts { get; }
}
using System.Data;
using Dapper;
using Template.Data.Domain.Entities;
using Template.Data.Domain.Repositories;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Entities;

namespace Template.Data.Infrastructure.Repositories;

internal class WeatherForecastsRepository : Repository<WeatherForecast, WeatherForecastPersistentEntity, int>,
    IWeatherForecastsRepository
{
    public WeatherForecastsRepository(IDbConnection connection, IDbTransaction transaction)
        : base(connection, transaction)
    {
    }

    public async Task<List<WeatherForecast>> GetForecastsAsync(CancellationToken cancellationToken)
    {
        var cmd = new CommandDefinition($"select * from {TableName} limit 100",
            transaction: Transaction,
            cancellationToken: cancellationToken);

        var forecasts = await Connection.QueryAsync<WeatherForecastPersistentEntity>(cmd);

        return forecasts
            .Select(MapToDomainEntity)
            .ToList();
    }

    protected override WeatherForecastPersistentEntity MapToPersistentEntity(WeatherForecast entity)
    {
        return new WeatherForecastPersistentEntity
        {
            Id = entity.Id,
            Date = entity.Date,
            TemperatureC = entity.TemperatureC
        };
    }

    protected override WeatherForecast MapToDomainEntity(WeatherForecastPersistentEntity entity)
    {
        return new WeatherForecast(entity.Id)
            .SetDate(entity.Date)
            .SetCelciusTemperature(entity.TemperatureC);
    }

    protected override void SetPersistentEntityId(WeatherForecastPersistentEntity entity, int id)
    {
        entity.Id = id;
    }
}
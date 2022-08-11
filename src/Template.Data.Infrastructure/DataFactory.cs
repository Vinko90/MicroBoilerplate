using System.Data;
using Npgsql;
using Template.Data.Domain.Interfaces;
using Template.Data.Domain.Repositories;
using Template.Data.Infrastructure.Repositories;

namespace Template.Data.Infrastructure;

public class DataFactory : IDataFactory, IDisposable
{
    private readonly IDbConnection _connection;
    private IDbTransaction _transaction;
        
    public IWeatherForecastsRepository WeatherForecasts { get; private set; }

    public DataFactory(string dbConnectionString)
    {
        _connection = new NpgsqlConnection(dbConnectionString);
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        WeatherForecasts = new WeatherForecastsRepository(_connection, _transaction);
    }
    
    public Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            _transaction.Commit();
        }
        catch
        {
            _transaction.Rollback();
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = _connection.BeginTransaction();
            WeatherForecasts = new WeatherForecastsRepository(_connection, _transaction);
        }
            
        return Task.CompletedTask;
    }
    
    public void Dispose()
    {
        _transaction.Dispose();
        _connection.Dispose();
    }
}
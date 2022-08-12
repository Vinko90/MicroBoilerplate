using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Template.Data.Infrastructure.Abstractions;
using Template.Data.Infrastructure.Repositories.Integration;
using Template.Data.Infrastructure.Repositories.Interfaces;

namespace Template.Data.Infrastructure.Extensions;

public static class InfrastructureDataExtensions
{
    public static void AddDataInfrastructure(this IServiceCollection services)
    {
        RepoDb.PostgreSqlBootstrap.Initialize();
        
        //UoW Transient
        services.AddTransient<IUnitOfWork<NpgsqlConnection>, DbUnitOfWork>();
        
        //Repo Singleton
        services.AddSingleton<IWeatherForecastRepository, WeatherForecastRepository>();
    }
}
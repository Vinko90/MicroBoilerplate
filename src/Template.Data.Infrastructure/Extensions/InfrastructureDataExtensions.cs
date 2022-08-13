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
        services.AddTransient<IUnitOfWork<NpgsqlConnection>, UnitOfWork>();
        
        //Repo Singleton
        services.AddSingleton<IRolesRepository, RolesRepository>();
        services.AddSingleton<IUserRolesRepository, UserRolesRepository>();
        services.AddSingleton<IUsersRepository, UsersRepository>();
        services.AddSingleton<IUserTokensRepository, UserTokensRepository>();
    }
}
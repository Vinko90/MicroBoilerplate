using Microsoft.Extensions.DependencyInjection;
using Template.Data.Domain.Interfaces;

namespace Template.Data.Infrastructure.Extensions;

public static class InfrastructureDataExtensions
{
    public static void AddInfrastructureDapper(this IServiceCollection services)
    {
        services.AddScoped<IDataFactory, DataFactory>();
    }
}
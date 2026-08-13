using Microsoft.Extensions.DependencyInjection;
using GarageHub.Domain.Interfaces;
using GarageHub.Infrastructure.Repositories;
using GarageHub.Infrastructure.Database;
using Microsoft.Extensions.Configuration;

namespace GarageHub.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Connection string 'DefaultConnection' não encontrada.");

        services.AddSingleton(new DbConnectionFactory(connectionString));

        services.AddScoped<VehicleRepository>();
        return services;
    }
}
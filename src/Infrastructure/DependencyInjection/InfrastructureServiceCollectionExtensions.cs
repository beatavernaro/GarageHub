using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Infrastructure.Database;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' não encontrada.");

        services.AddSingleton<IDbConnectionFactory>(new DbConnectionFactory(connectionString));

        services.AddSingleton<SqlFileReader>();
        services.AddScoped<ICurrentUser, CurrentUser>();

        services.AddScoped<IClienteRepository, ClienteRepository>();
        services.AddScoped<IClienteService, ClienteService>();

        services.AddScoped<IVeiculoRepository, VeiculoRepository>();
        services.AddScoped<IVeiculoService, VeiculoService>();

        services.AddScoped<IItemEstoqueRepository, ItemEstoqueRepository>();
        services.AddScoped<IItemEstoqueService, ItemEstoqueService>();

        services.AddScoped<IServicoRepository, ServicoRepository>();
        services.AddScoped<IServicoService, ServicoService>();
        return services;
    }
}
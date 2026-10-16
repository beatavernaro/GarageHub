using Application.Interfaces;
using Dapper;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Api.HealthChecks;

// Verifica se o PostgreSQL está acessível. Usado pela readiness probe do Kubernetes
// para que o pod só receba tráfego quando conseguir se comunicar com o banco.
public sealed class DatabaseHealthCheck(
    IDbConnectionFactory connectionFactory) : IHealthCheck
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = _connectionFactory.CreateConnection();

            await connection.ExecuteScalarAsync<int>(
                new CommandDefinition(
                    "SELECT 1",
                    cancellationToken: cancellationToken));

            return HealthCheckResult.Healthy();
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Banco de dados indisponível.",
                ex);
        }
    }
}

using GarageHub.Infrastructure.Database;

namespace GarageHub.Infrastructure.Repositories;

public class VehicleRepository(DbConnectionFactory connectionFactory)
{
    public async Task<bool> TestConnectionAsync()
    {
        await using var connection = connectionFactory.CreateConnection();

        await connection.OpenAsync();

        return connection.State == System.Data.ConnectionState.Open;
    }
}
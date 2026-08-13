using Npgsql;

namespace GarageHub.Infrastructure.Database;

public class DbConnectionFactory(string connectionString)
{
    public NpgsqlConnection CreateConnection()
        => new(connectionString);
}
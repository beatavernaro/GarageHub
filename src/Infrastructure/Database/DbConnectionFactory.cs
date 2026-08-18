using Application.Interfaces;
using Npgsql;
using System.Data;

namespace Infrastructure.Database;

public class DbConnectionFactory(string connectionString) : IDbConnectionFactory
{
    public IDbConnection CreateConnection()
    {
        return new NpgsqlConnection(connectionString);
    }
}
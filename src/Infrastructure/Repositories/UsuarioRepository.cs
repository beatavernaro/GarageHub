using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class UsuarioRepository(
    IDbConnectionFactory connectionFactory,
    SqlFileReader sqlFileReader)
    : IUsuarioRepository
{
    private readonly IDbConnectionFactory _connectionFactory =
        connectionFactory;

    private readonly SqlFileReader _sqlFileReader =
        sqlFileReader;

    public async Task<Usuario?> ObterPorEmailAsync(
        string email)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get(
                "Usuario/ObterPorEmail.sql");

        var model =
            await connection
                .QuerySingleOrDefaultAsync<UsuarioDbModel>(
                    sql,
                    new { Email = email });

        return model?.ToEntity();
    }
}
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class ClienteRepository(IDbConnectionFactory connectionFactory, SqlFileReader sqlFileReader) : IClienteRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly SqlFileReader _sqlFileReader = sqlFileReader;

    public async Task<Cliente?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Cliente/ObterPorId.sql");

        var model = await connection.QuerySingleOrDefaultAsync<ClienteDbModel>(sql, new { Id = id });

        return model?.ToEntity();
    }

    public async Task<Cliente?> ObterPorDocumentoAsync(string documento)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Cliente/ObterPorDocumento.sql");

        var model = await connection.QuerySingleOrDefaultAsync<ClienteDbModel>(sql, new { Documento = documento });

        return model?.ToEntity();
    }

    public async Task<IEnumerable<Cliente>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Cliente/ObterTodos.sql");

        var models = await connection.QueryAsync<ClienteDbModel>(sql);

        return models.Select(x => x.ToEntity());
    }

    public async Task AdicionarAsync(Cliente cliente)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Cliente/Adicionar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(cliente));
    }

    public async Task AtualizarAsync(Cliente cliente)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Cliente/Atualizar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(cliente));
    }

    private static object MapearParametros(Cliente cliente)
    {
        return new
        {
            cliente.Id,
            cliente.Nome,
            cliente.Documento,
            TipoPessoa = (int)cliente.TipoPessoa,
            cliente.Telefone,
            cliente.Email,

            cliente.Endereco?.Logradouro,
            cliente.Endereco?.Numero,
            cliente.Endereco?.Complemento,
            cliente.Endereco?.Bairro,
            cliente.Endereco?.Cidade,
            cliente.Endereco?.Estado,
            cliente.Endereco?.Cep,

            cliente.CriadoPorId,
            cliente.DataCriacao,
            cliente.DataAlteracao,
            cliente.AlteradoPorId,
            cliente.Ativo
        };
    }
}
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class VeiculoRepository(
    IDbConnectionFactory connectionFactory,
    SqlFileReader sqlFileReader) : IVeiculoRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly SqlFileReader _sqlFileReader = sqlFileReader;

    public async Task<Veiculo?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/ObterPorId.sql");

        var model = await connection.QuerySingleOrDefaultAsync<VeiculoDbModel>(
            sql,
            new { Id = id });

        return model?.ToEntity();
    }

    public async Task<IEnumerable<Veiculo>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/ObterTodos.sql");

        var models = await connection.QueryAsync<VeiculoDbModel>(sql);

        return models.Select(x => x.ToEntity());
    }

    public async Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(Guid clienteId)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/ObterPorClienteId.sql");

        var models = await connection.QueryAsync<VeiculoDbModel>(
            sql,
            new { ClienteId = clienteId });

        return models.Select(x => x.ToEntity());
    }

    public async Task<Veiculo?> ObterPorPlacaAsync(string placa)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/ObterPorPlaca.sql");

        var model = await connection.QuerySingleOrDefaultAsync<VeiculoDbModel>(
            sql,
            new { Placa = placa });

        return model?.ToEntity();
    }

    public async Task AdicionarAsync(Veiculo veiculo)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/Adicionar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(veiculo));
    }

    public async Task AtualizarAsync(Veiculo veiculo)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/Atualizar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(veiculo));
    }

    public async Task DesativarAsync(Veiculo veiculo)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Veiculo/Desativar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(veiculo));
    }

    private static object MapearParametros(Veiculo veiculo)
    {
        return new
        {
            veiculo.Id,
            veiculo.ClienteId,
            veiculo.Placa,
            veiculo.Chassi,
            veiculo.Marca,
            veiculo.Modelo,
            veiculo.Cor,
            veiculo.Ano,
            veiculo.Quilometragem,
            veiculo.CriadoPorId,
            veiculo.DataCriacao,
            veiculo.DataAlteracao,
            veiculo.AlteradoPorId,
            veiculo.Ativo
        };
    }
}
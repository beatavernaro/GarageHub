using Application.DTOs.Servico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class ServicoRepository(IDbConnectionFactory connectionFactory, SqlFileReader sqlFileReader) : IServicoRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    private readonly SqlFileReader _sqlFileReader = sqlFileReader;

    public async Task<Servico?> ObterPorIdAsync(Guid id)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Servico/ObterPorId.sql");

        var model =
            await connection.QuerySingleOrDefaultAsync<ServicoDbModel>(
                sql,
                new { Id = id });

        return model?.ToEntity();
    }

    public async Task<IEnumerable<Servico>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/ObterTodos.sql");

        var models = await connection.QueryAsync<ServicoDbModel>(sql);

        return models.Select(x => x.ToEntity());
    }

    public async Task<Servico?> ObterPorCodigoInternoAsync(string codigoInterno)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/ObterPorCodigoInterno.sql");

        var model = await connection.QuerySingleOrDefaultAsync<ServicoDbModel>(sql, new { CodigoInterno = codigoInterno });

        return model?.ToEntity();
    }

    public async Task AdicionarAsync(Servico servico)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/Adicionar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(servico));
    }

    public async Task AtualizarAsync(Servico servico)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/Atualizar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(servico));
    }

    public async Task<IEnumerable<TempoMedioServicoDto>>
    ObterTemposMediosAsync()
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get(
                "Servico/ObterTemposMedios.sql");

        return await connection
            .QueryAsync<TempoMedioServicoDto>(sql);
    }

    private static object MapearParametros(Servico servico)
    {
        return new
        {
            servico.Id,
            servico.CodigoInterno,
            servico.Nome,
            servico.Descricao,
            servico.Preco,
            servico.CriadoPorId,
            servico.DataCriacao,
            servico.DataAlteracao,
            servico.AlteradoPorId,
            servico.Ativo
        };
    }
}
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class ItemEstoqueRepository(IDbConnectionFactory connectionFactory, SqlFileReader sqlFileReader) : IItemEstoqueRepository
{
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;
    private readonly SqlFileReader _sqlFileReader = sqlFileReader;

    public async Task<ItemEstoque?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("ItemEstoque/ObterPorId.sql");

        var model = await connection.QuerySingleOrDefaultAsync<ItemEstoqueDbModel>(
            sql,
            new { Id = id });

        return model?.ToEntity();
    }

    public async Task<ItemEstoque?> ObterPorCodigoInternoAsync(
        string codigoInterno)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("ItemEstoque/ObterPorCodigoInterno.sql");

        var model = await connection.QuerySingleOrDefaultAsync<ItemEstoqueDbModel>(
            sql,
            new { CodigoInterno = codigoInterno });

        return model?.ToEntity();
    }

    public async Task<IEnumerable<ItemEstoque>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("ItemEstoque/ObterTodos.sql");

        var models = await connection.QueryAsync<ItemEstoqueDbModel>(sql);

        return models.Select(x => x.ToEntity());
    }

    public async Task AdicionarAsync(ItemEstoque item)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("ItemEstoque/Adicionar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(item));
    }

    public async Task AtualizarAsync(ItemEstoque item)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("ItemEstoque/Atualizar.sql");

        await connection.ExecuteAsync(sql, MapearParametros(item));
    }

    private static object MapearParametros(ItemEstoque item)
    {
        return new
        {
            item.Id,
            item.CodigoInterno,
            item.Nome,
            item.Descricao,
            Tipo = (int)item.Tipo,
            item.Preco,
            item.Estoque,
            item.CriadoPorId,
            item.DataCriacao,
            item.DataAlteracao,
            item.AlteradoPorId,
            item.Ativo
        };
    }
}
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class ServicoRepository(
    IDbConnectionFactory connectionFactory,
    SqlFileReader sqlFileReader) : IServicoRepository
{
    private readonly IDbConnectionFactory _connectionFactory =
        connectionFactory;

    private readonly SqlFileReader _sqlFileReader =
        sqlFileReader;

    public async Task<Servico?> ObterPorIdAsync(Guid id)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/ObterPorId.sql");

        using var multi = await connection.QueryMultipleAsync(
            sql,
            new { Id = id });

        var model =
            await multi.ReadSingleOrDefaultAsync<ServicoDbModel>();

        if (model is null)
            return null;

        model.ItensEstoque =
        [
            .. await multi.ReadAsync<ServicoItemEstoqueDbModel>()
        ];

        return model.ToEntity();
    }

    public async Task<IEnumerable<Servico>> ObterTodosAsync()
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/ObterTodos.sql");

        var models =
            await connection.QueryAsync<ServicoDbModel>(sql);

        return models.Select(x => x.ToEntity());
    }

    public async Task<Servico?> ObterPorNomeAsync(string nome)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/ObterPorNome.sql");

        var model =
            await connection.QuerySingleOrDefaultAsync<ServicoDbModel>(
                sql,
                new { Nome = nome });

        return model?.ToEntity();
    }

    public async Task AdicionarAsync(Servico servico)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/Adicionar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(servico));
    }

    public async Task AtualizarAsync(Servico servico)
    {
        using var connection = _connectionFactory.CreateConnection();

        var sql = _sqlFileReader.Get("Servico/Atualizar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(servico));
    }

    public async Task AtualizarItensEstoqueAsync(Servico servico)
    {
        using var connection = _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction = connection.BeginTransaction();

        try
        {
            var itensAtivos = servico.ItensEstoque
                .Where(x => x.Ativo)
                .ToList();

            var idsAtivos = itensAtivos
                .Select(x => x.Id)
                .ToList();

            var sqlItensExistentes =
                _sqlFileReader.Get("Servico/ObterItensEstoque.sql");

            var itensExistentes =
                (
                    await connection.QueryAsync<Guid>(
                        sqlItensExistentes,
                        new { ServicoId = servico.Id },
                        transaction)
                ).ToList();

            var sqlInativar =
                _sqlFileReader.Get("Servico/InativarItemEstoque.sql");

            foreach (var id in itensExistentes)
            {
                if (!idsAtivos.Contains(id))
                {
                    await connection.ExecuteAsync(
                        sqlInativar,
                        new
                        {
                            Id = id,
                            DataAlteracao = servico.DataAlteracao,
                            AlteradoPorId = servico.AlteradoPorId
                        },
                        transaction);
                }
            }

            var sqlAdicionar =
                _sqlFileReader.Get("Servico/AdicionarItemEstoque.sql");

            var sqlAtualizar =
                _sqlFileReader.Get("Servico/AtualizarItemEstoque.sql");

            foreach (var item in itensAtivos)
            {
                if (itensExistentes.Contains(item.Id))
                {
                    await connection.ExecuteAsync(
                        sqlAtualizar,
                        new
                        {
                            item.Id,
                            item.Quantidade,
                            item.DataAlteracao,
                            item.AlteradoPorId,
                            item.Ativo
                        },
                        transaction);
                }
                else
                {
                    await connection.ExecuteAsync(
                        sqlAdicionar,
                        new
                        {
                            item.Id,
                            item.ServicoId,
                            item.ItemEstoqueId,
                            item.Quantidade,
                            item.CriadoPorId,
                            item.DataCriacao,
                            item.DataAlteracao,
                            item.AlteradoPorId,
                            item.Ativo
                        },
                        transaction);
                }
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    private static object MapearParametros(Servico servico)
    {
        return new
        {
            servico.Id,
            servico.Nome,
            servico.Descricao,
            servico.Preco,
            Status = (int)servico.Status,
            servico.CriadoPorId,
            servico.DataCriacao,
            servico.DataAlteracao,
            servico.AlteradoPorId,
            servico.Ativo
        };
    }
}
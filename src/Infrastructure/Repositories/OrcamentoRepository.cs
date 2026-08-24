using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class OrcamentoRepository(
    IDbConnectionFactory connectionFactory,
    SqlFileReader sqlFileReader) : IOrcamentoRepository
{
    private readonly IDbConnectionFactory _connectionFactory =
        connectionFactory;

    private readonly SqlFileReader _sqlFileReader =
        sqlFileReader;

    public async Task<Orcamento?> ObterPorIdAsync(Guid id)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Orcamento/ObterPorId.sql");

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                new { Id = id });

        var model =
            await multi.ReadSingleOrDefaultAsync<OrcamentoDbModel>();

        if (model is null)
            return null;

        model.Itens =
        [
            .. await multi.ReadAsync<OrcamentoItemDbModel>()
        ];

        return model.ToEntity();
    }

    public async Task<IEnumerable<Orcamento>> ObterTodosAsync()
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Orcamento/ObterTodos.sql");

        using var multi =
            await connection.QueryMultipleAsync(sql);

        var orcamentos =
            (await multi.ReadAsync<OrcamentoDbModel>()).ToList();

        var itens =
            (await multi.ReadAsync<OrcamentoItemDbModel>()).ToList();

        foreach (var orcamento in orcamentos)
        {
            orcamento.Itens =
            [
                .. itens.Where(x => x.OrcamentoId == orcamento.Id)
            ];
        }

        return orcamentos.Select(x => x.ToEntity());
    }

    public async Task<IEnumerable<Orcamento>> ObterPorClienteIdAsync(
        Guid clienteId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Orcamento/ObterPorClienteId.sql");

        var models =
            await connection.QueryAsync<OrcamentoDbModel>(
                sql,
                new { ClienteId = clienteId });

        return models.Select(x => x.ToEntity());
    }

    public async Task AdicionarAsync(Orcamento orcamento)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Orcamento/Adicionar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(orcamento));
    }

    public async Task AtualizarAsync(Orcamento orcamento)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("Orcamento/Atualizar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(orcamento));
    }

    public async Task AtualizarItensAsync(
    Orcamento orcamento)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction =
            connection.BeginTransaction();

        try
        {
            var itensAtivos = orcamento.Itens
                .Where(x => x.Ativo)
                .ToList();

            var idsAtivos = itensAtivos
                .Select(x => x.Id)
                .ToHashSet();

            var sqlItensExistentes =
                _sqlFileReader.Get(
                    "Orcamento/ObterIdsItens.sql");

            var idsExistentes =
                (
                    await connection.QueryAsync<Guid>(
                        sqlItensExistentes,
                        new { OrcamentoId = orcamento.Id },
                        transaction)
                ).ToHashSet();

            var sqlInativar =
                _sqlFileReader.Get(
                    "Orcamento/InativarItem.sql");

            foreach (var id in idsExistentes)
            {
                if (!idsAtivos.Contains(id))
                {
                    var item = orcamento.Itens
                        .FirstOrDefault(x => x.Id == id);

                    await connection.ExecuteAsync(
                        sqlInativar,
                        new
                        {
                            Id = id,
                            DataAlteracao =
                                item?.DataAlteracao
                                ?? orcamento.DataAlteracao,
                            AlteradoPorId =
                                item?.AlteradoPorId
                                ?? orcamento.AlteradoPorId
                        },
                        transaction);
                }
            }

            var sqlAdicionar =
                _sqlFileReader.Get(
                    "Orcamento/AdicionarItem.sql");

            var sqlAtualizar =
                _sqlFileReader.Get(
                    "Orcamento/AtualizarItem.sql");

            foreach (var item in itensAtivos)
            {
                if (idsExistentes.Contains(item.Id))
                {
                    await connection.ExecuteAsync(
                        sqlAtualizar,
                        new
                        {
                            item.Id,
                            item.Quantidade,
                            item.ValorUnitario,
                            item.ValorTotal,
                            item.DataAlteracao,
                            item.AlteradoPorId,
                            item.Ativo
                        },
                        transaction);
                }
                else
                {
                    await connection.ExecuteAsync(sqlAdicionar, new
                    {
                        item.Id,
                        item.OrcamentoId,
                        item.ServicoId,
                        item.ItemEstoqueId,
                        item.NomeItem,
                        item.DescricaoItem,
                        item.Quantidade,
                        item.ValorUnitario,
                        item.ValorTotal,
                        item.CriadoPorId,
                        item.DataCriacao,
                        item.DataAlteracao,
                        item.AlteradoPorId,
                        item.Ativo
                    }, transaction);
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

    private static object MapearParametros(
        Orcamento orcamento)
    {
        return new
        {
            orcamento.Id,
            orcamento.ClienteId,
            orcamento.VeiculoId,
            Status = (int)orcamento.Status,
            orcamento.Desconto,
            orcamento.ValorTotal,
            orcamento.DataEnvioCliente,
            orcamento.DataAprovacao,
            orcamento.DataRejeicao,
            orcamento.CriadoPorId,
            orcamento.DataCriacao,
            orcamento.DataAlteracao,
            orcamento.AlteradoPorId,
            orcamento.Ativo
        };
    }
}
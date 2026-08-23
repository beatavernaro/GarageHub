using Application.Interfaces;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Mappers;
using Infrastructure.Models;

namespace Infrastructure.Repositories;

public class OrdemServicoRepository(
    IDbConnectionFactory connectionFactory,
    SqlFileReader sqlFileReader) : IOrdemServicoRepository
{
    private readonly IDbConnectionFactory _connectionFactory =
        connectionFactory;

    private readonly SqlFileReader _sqlFileReader =
        sqlFileReader;

    public async Task<OrdemServico?> ObterPorIdAsync(Guid id)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("OrdemServico/ObterPorId.sql");

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                new { Id = id });

        var model =
            await multi.ReadSingleOrDefaultAsync<OrdemServicoDbModel>();

        if (model is null)
            return null;

        model.Itens =
        [
            .. await multi.ReadAsync<OrdemServicoItemEstoqueDbModel>()
        ];

        model.Servicos =
        [
            .. await multi.ReadAsync<OrdemServicoServicoDbModel>()
        ];

        return model.ToEntity();
    }

    public async Task<IEnumerable<OrdemServico>> ObterTodosAsync()
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("OrdemServico/ObterTodos.sql");

        using var multi =
            await connection.QueryMultipleAsync(sql);

        var ordens =
            (await multi.ReadAsync<OrdemServicoDbModel>())
            .ToList();

        var itens =
            (await multi.ReadAsync<OrdemServicoItemEstoqueDbModel>())
            .ToList();

        var servicos =
            (await multi.ReadAsync<OrdemServicoServicoDbModel>())
            .ToList();

        foreach (var ordem in ordens)
        {
            ordem.Itens =
            [
                .. itens.Where(x =>
                    x.OrdemServicoId == ordem.Id)
            ];

            ordem.Servicos =
            [
                .. servicos.Where(x =>
                    x.OrdemServicoId == ordem.Id)
            ];
        }

        return ordens.Select(x => x.ToEntity());
    }

    public async Task AdicionarAsync(
        OrdemServico ordemServico)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("OrdemServico/Adicionar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(ordemServico));

        await AdicionarItensAsync(ordemServico);
    }

    public async Task AtualizarAsync(
        OrdemServico ordemServico)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get("OrdemServico/Atualizar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(ordemServico));
    }

    public async Task AdicionarItensAsync(
        OrdemServico ordemServico)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        connection.Open();

        using var transaction =
            connection.BeginTransaction();

        try
        {
            var sqlItem =
                _sqlFileReader.Get(
                    "OrdemServico/AdicionarItemEstoque.sql");

            foreach (var item in ordemServico.Itens.Where(x => x.Ativo))
            {
                await connection.ExecuteAsync(
                    sqlItem,
                    new
                    {
                        item.Id,
                        item.OrdemServicoId,
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
                    },
                    transaction);
            }

            var sqlServico =
                _sqlFileReader.Get(
                    "OrdemServico/AdicionarServico.sql");

            foreach (var servico in ordemServico.Servicos.Where(x => x.Ativo))
            {
                await connection.ExecuteAsync(
                    sqlServico,
                    new
                    {
                        servico.Id,
                        servico.OrdemServicoId,
                        servico.ServicoId,
                        servico.NomeServico,
                        servico.DescricaoServico,
                        servico.Quantidade,
                        servico.ValorUnitario,
                        servico.ValorTotal,
                        Status = (int)servico.Status,
                        servico.CriadoPorId,
                        servico.DataCriacao,
                        servico.DataAlteracao,
                        servico.AlteradoPorId,
                        servico.Ativo
                    },
                    transaction);
            }

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    public async Task AtualizarServicoStatusAsync(
        Guid ordemServicoId,
        Guid servicoId,
        StatusServico status,
        Guid usuarioId)
    {
        using var connection =
            _connectionFactory.CreateConnection();

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/AtualizarServicoStatus.sql");

        await connection.ExecuteAsync(
            sql,
            new
            {
                OrdemServicoId = ordemServicoId,
                ServicoId = servicoId,
                Status = (int)status,
                AlteradoPorId = usuarioId
            });
    }

    private static object MapearParametros(
        OrdemServico ordemServico)
    {
        return new
        {
            ordemServico.Id,
            ordemServico.OrcamentoId,
            ordemServico.ClienteId,
            ordemServico.VeiculoId,
            Status = (int)ordemServico.Status,
            ordemServico.Desconto,
            ordemServico.ValorTotal,
            ordemServico.DataInicio,
            ordemServico.DataFinalizacao,
            ordemServico.DataEntrega,
            ordemServico.CriadoPorId,
            ordemServico.DataCriacao,
            ordemServico.DataAlteracao,
            ordemServico.AlteradoPorId,
            ordemServico.Ativo
        };
    }
}
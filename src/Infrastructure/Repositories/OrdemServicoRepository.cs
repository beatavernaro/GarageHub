using Application.DTOs.OrdemServico;
using Application.Interfaces.Repositories;
using Dapper;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Database;
using Infrastructure.Mappers;

namespace Infrastructure.Repositories;

public class OrdemServicoRepository(
    DbSession dbSession,
    SqlFileReader sqlFileReader)
    : IOrdemServicoRepository
{
    private readonly DbSession _dbSession =
        dbSession;

    private readonly SqlFileReader _sqlFileReader =
        sqlFileReader;

    public async Task<OrdemServico?> ObterPorIdAsync(Guid id)
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/ObterPorId.sql");

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                new { Id = id },
                conexao.Transaction);

        var model =
            await multi.ReadSingleOrDefaultAsync<OrdemServicoDbModel>();

        if (model is null)
            return null;

        model.Servicos =
        [
            .. await multi.ReadAsync<OrdemServicoServicoDbModel>()
        ];

        model.Itens =
        [
            .. await multi.ReadAsync<OrdemServicoItemEstoqueDbModel>()
        ];

        return model.ToEntity();
    }

    public async Task<IEnumerable<OrdemServico>> ObterTodosAsync()
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/ObterTodos.sql");

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                transaction: conexao.Transaction);

        var ordens =
            (await multi.ReadAsync<OrdemServicoDbModel>())
            .ToList();

        var servicos =
            (await multi.ReadAsync<OrdemServicoServicoDbModel>())
            .ToList();

        var itens =
            (await multi.ReadAsync<OrdemServicoItemEstoqueDbModel>())
            .ToList();

        foreach (var ordem in ordens)
        {
            ordem.Servicos =
            [
                .. servicos.Where(x =>
                    x.OrdemServicoId == ordem.Id)
            ];

            ordem.Itens =
            [
                .. itens.Where(x =>
                    x.OrdemServicoId == ordem.Id)
            ];
        }

        return ordens.Select(x => x.ToEntity());
    }

    public async Task<OrdemServico?> ObterAtualPorPlacaAsync(
    string placa)
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/ObterAtualPorPlaca.sql");

        using var multi =
            await connection.QueryMultipleAsync(
                sql,
                new { Placa = placa },
                conexao.Transaction);

        var model =
            await multi.ReadSingleOrDefaultAsync<OrdemServicoDbModel>();

        if (model is null)
            return null;

        model.Servicos =
        [
            .. await multi.ReadAsync<OrdemServicoServicoDbModel>()
        ];

        return model.ToEntity();
    }

    public async Task AdicionarAsync(
        OrdemServico ordemServico)
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var transaction =
            conexao.IniciarTransacao();

        try
        {
            var sqlOrdem =
                _sqlFileReader.Get(
                    "OrdemServico/Adicionar.sql");

            await connection.ExecuteAsync(
                sqlOrdem,
                MapearParametros(ordemServico),
                transaction);

            var sqlServico =
                _sqlFileReader.Get(
                    "OrdemServico/AdicionarServico.sql");

            foreach (var servico in ordemServico.Servicos)
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
                        servico.DataInicio,
                        servico.DataFinalizacao,
                        servico.CriadoPorId,
                        servico.DataCriacao,
                        servico.DataAlteracao,
                        servico.AlteradoPorId,
                        servico.Ativo
                    },
                    transaction);
            }

            var sqlItem =
                _sqlFileReader.Get(
                    "OrdemServico/AdicionarItemEstoque.sql");

            foreach (var item in ordemServico.Itens)
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

            conexao.Confirmar();
        }
        catch
        {
            conexao.Desfazer();
            throw;
        }
    }

    public async Task AtualizarAsync(
        OrdemServico ordemServico)
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/Atualizar.sql");

        await connection.ExecuteAsync(
            sql,
            MapearParametros(ordemServico),
            conexao.Transaction);
    }

    public async Task AtualizarServicoStatusAsync(
    OrdemServicoServico servico)
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/AtualizarServicoStatus.sql");

        await connection.ExecuteAsync(
            sql,
            new
            {
                servico.Id,
                servico.OrdemServicoId,
                Status = (int)servico.Status,
                servico.DataInicio,
                servico.DataFinalizacao,
                servico.DataAlteracao,
                servico.AlteradoPorId
            },
            conexao.Transaction);
    }

    public async Task<IEnumerable<TempoOrdemServicoDto>> ObterTemposOrdensAsync()
    {
        using var conexao =
            _dbSession.ObterConexao();

        var connection = conexao.Connection;

        var sql =
            _sqlFileReader.Get(
                "OrdemServico/ObterTemposOrdens.sql");

        return await connection
            .QueryAsync<TempoOrdemServicoDto>(
                sql,
                transaction: conexao.Transaction);
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
using Domain.Entities;
using Infrastructure.Models;

namespace Infrastructure.Mappers;

public static class OrdemServicoMapper
{
    public static OrdemServico ToEntity(
        this OrdemServicoDbModel model)
    {
        var itens = model.Itens
            .Select(x => new OrdemServicoItemEstoque(
                x.Id,
                x.DataCriacao,
                x.CriadoPorId,
                x.DataAlteracao,
                x.AlteradoPorId,
                x.Ativo,
                x.OrdemServicoId,
                x.ItemEstoqueId,
                x.NomeItem,
                x.DescricaoItem,
                x.Quantidade,
                x.ValorUnitario))
            .ToList();

        var servicos = model.Servicos
            .Select(x => new OrdemServicoServico(
                x.Id,
                x.DataCriacao,
                x.CriadoPorId,
                x.DataAlteracao,
                x.AlteradoPorId,
                x.Ativo,
                x.OrdemServicoId,
                x.ServicoId,
                x.NomeServico,
                x.DescricaoServico,
                x.Quantidade,
                x.ValorUnitario,
                x.Status))
            .ToList();

        return new OrdemServico(
            model.Id,
            model.DataCriacao,
            model.CriadoPorId,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo,
            model.OrcamentoId,
            model.ClienteId,
            model.VeiculoId,
            (Domain.Enums.StatusOrdemServico)model.Status,
            model.Desconto,
            model.ValorTotal,
            model.DataInicio,
            model.DataFinalizacao,
            model.DataEntrega,
            itens,
            servicos);
    }
}
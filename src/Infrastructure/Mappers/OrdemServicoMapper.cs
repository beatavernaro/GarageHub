using Domain.Entities;

namespace Infrastructure.Mappers;

public static class OrdemServicoMapper
{
    public static OrdemServico ToEntity(
        this OrdemServicoDbModel model)
    {
        var itens = model.Itens
            .Select(x => new OrdemServicoItemEstoque(
                x.Id,
                x.OrdemServicoId,
                x.ItemEstoqueId,
                x.NomeItem,
                x.DescricaoItem,
                x.Quantidade,
                x.ValorUnitario,
                x.ValorTotal,
                x.CriadoPorId,
                x.DataCriacao,
                x.DataAlteracao,
                x.AlteradoPorId,
                x.Ativo))
            .ToList();

        var servicos = model.Servicos
            .Select(x => new OrdemServicoServico(
                x.Id,
                x.OrdemServicoId,
                x.ServicoId,
                x.NomeServico,
                x.DescricaoServico,
                x.Quantidade,
                x.ValorUnitario,
                x.ValorTotal,
                x.Status,
                x.CriadoPorId,
                x.DataCriacao,
                x.DataAlteracao,
                x.AlteradoPorId,
                x.Ativo))
            .ToList();

        return new OrdemServico(
            model.Id,
            model.OrcamentoId,
            model.ClienteId,
            model.VeiculoId,
            model.Status,
            model.Desconto,
            model.ValorTotal,
            model.DataInicio,
            model.DataFinalizacao,
            model.DataEntrega,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo,
            itens,
            servicos);
    }
}
using Domain.Entities;

namespace Infrastructure.Mappers;

public static class OrcamentoMapper
{
    public static Orcamento ToEntity(this OrcamentoDbModel model)
    {
        var orcamento = new Orcamento(
            model.Id,
            model.ClienteId,
            model.VeiculoId,
            model.Status,
            model.Desconto,
            model.ValorTotal,
            model.DataEnvioCliente,
            model.DataAprovacao,
            model.DataRejeicao,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);

        orcamento.CarregarItens(
            model.Itens.Select(x => x.ToEntity()));

        return orcamento;
    }

    public static OrcamentoItem ToEntity(
        this OrcamentoItemDbModel model)
    {
        return new OrcamentoItem(
            model.Id,
            model.OrcamentoId,
            model.ServicoId,
            model.ItemEstoqueId,
            model.Quantidade,
            model.ValorUnitario,
            model.ValorTotal,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);
    }
}
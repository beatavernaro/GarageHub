using Domain.Entities;

namespace Infrastructure.Mappers;

public static class ServicoMapper
{
    public static Servico ToEntity(this ServicoDbModel model)
    {
        var servico = new Servico(
            model.Id,
            model.Nome,
            model.Descricao,
            model.Preco,
            model.Status,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);

        var itens = model.ItensEstoque
            .Select(x => x.ToEntity())
            .ToList();

        servico.CarregarItensEstoque(itens);

        return servico;
    }

    public static ServicoItemEstoque ToEntity(
        this ServicoItemEstoqueDbModel model)
    {
        return new ServicoItemEstoque(
            model.Id,
            model.ServicoId,
            model.ItemEstoqueId,
            model.Quantidade,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);
    }
}
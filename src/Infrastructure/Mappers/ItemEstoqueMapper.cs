using Domain.Entities;

namespace Infrastructure.Mappers;

public static class ItemEstoqueMapper
{
    public static ItemEstoque ToEntity(this ItemEstoqueDbModel model)
    {
        return new ItemEstoque(
            model.Id,
            model.CodigoInterno,
            model.Nome,
            model.Tipo,
            model.Preco,
            model.Estoque,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo,
            model.Descricao);
    }
}
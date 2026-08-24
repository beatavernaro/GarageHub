using Domain.Entities;
namespace Infrastructure.Mappers;
public static class ServicoMapper
{
    public static Servico ToEntity(this ServicoDbModel model)
    {
        var servico = new Servico(
            model.Id,
            model.CodigoInterno,
            model.Nome,
            model.Descricao,
            model.Preco,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);

        return servico;
    }
    
}
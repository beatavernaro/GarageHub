using Domain.Entities;

namespace Infrastructure.Mappers;

public static class UsuarioMapper
{
    public static Usuario ToEntity(
        this UsuarioDbModel model)
    {
        return new Usuario(
            model.Id,
            model.Nome,
            model.Email,
            model.SenhaHash,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);
    }
}
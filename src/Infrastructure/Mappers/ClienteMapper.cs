using Domain.Entities;
using Domain.ValueObjects;

namespace Infrastructure.Mappers;

public static class ClienteMapper
{
    public static Cliente ToEntity(this ClienteDbModel model)
    {
        Endereco? endereco = null;

        if (!string.IsNullOrWhiteSpace(model.Logradouro))
        {
            endereco = new Endereco(
                model.Logradouro,
                model.Numero ?? string.Empty,
                model.Complemento,
                model.Bairro ?? string.Empty,
                model.Cidade ?? string.Empty,
                model.Estado ?? string.Empty,
                model.Cep ?? string.Empty);
        }

        return new Cliente(
            model.Id,
            model.Nome,
            model.Documento,
            model.TipoPessoa,
            model.Telefone,
            model.Email,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo,
            endereco);
    }
}
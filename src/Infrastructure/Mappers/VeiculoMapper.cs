using Domain.Entities;

namespace Infrastructure.Mappers;

public static class VeiculoMapper
{
    public static Veiculo ToEntity(this VeiculoDbModel model)
    {
        return new Veiculo(
            model.Id,
            model.ClienteId,
            model.Placa,
            model.Chassi,
            model.Marca,
            model.Modelo,
            model.Cor,
            model.Ano,
            model.Quilometragem,
            model.CriadoPorId,
            model.DataCriacao,
            model.DataAlteracao,
            model.AlteradoPorId,
            model.Ativo);
    }
}
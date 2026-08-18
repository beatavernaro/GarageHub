namespace Infrastructure.Mappers;

public class VeiculoDbModel
{
    public Guid Id { get; init; }
    public Guid ClienteId { get; init; }

    public string Placa { get; init; } = string.Empty;
    public string? Chassi { get; init; }

    public string Marca { get; init; } = string.Empty;
    public string Modelo { get; init; } = string.Empty;
    public string Cor { get; init; } = string.Empty;

    public int Ano { get; init; }
    public int Quilometragem { get; init; }

    public Guid? CriadoPorId { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime? DataAlteracao { get; init; }
    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }
}
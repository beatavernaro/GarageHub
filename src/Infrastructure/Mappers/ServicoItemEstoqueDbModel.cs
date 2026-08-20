namespace Infrastructure.Mappers;

public class ServicoItemEstoqueDbModel
{
    public Guid Id { get; init; }

    public Guid ServicoId { get; init; }

    public Guid ItemEstoqueId { get; init; }

    public int Quantidade { get; init; }

    public Guid? CriadoPorId { get; init; }

    public DateTime DataCriacao { get; init; }

    public DateTime? DataAlteracao { get; init; }

    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }
}
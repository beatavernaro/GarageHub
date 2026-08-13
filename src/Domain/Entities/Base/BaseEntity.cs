namespace Domain.Entities.Base;

public abstract class BaseEntity
{
    public Guid Id { get; protected set; }

    public DateTime DataCriacao { get; protected set; }
    public Guid? CriadoPorId { get; protected set; }

    public DateTime? DataAlteracao { get; protected set; }
    public Guid? AlteradoPorId { get; protected set; }

    public bool Ativo { get; protected set; }
    public DateTime? DataInativacao { get; protected set; }
}
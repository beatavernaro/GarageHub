namespace Domain.Entities.Base;

public abstract class BaseEntity
{
    protected BaseEntity()
    {
    }

    protected BaseEntity(Guid? criadoPorId)
    {
        Id = Guid.NewGuid();
        DataCriacao = DateTime.UtcNow;
        CriadoPorId = criadoPorId;
        Ativo = true;
    }

    protected BaseEntity(
        Guid id,
        DateTime dataCriacao,
        Guid? criadoPorId,
        DateTime? dataAlteracao,
        Guid? alteradoPorId,
        bool ativo)
    {
        Id = id;
        DataCriacao = dataCriacao;
        CriadoPorId = criadoPorId;
        DataAlteracao = dataAlteracao;
        AlteradoPorId = alteradoPorId;
        Ativo = ativo;
    }

    public Guid Id { get; protected set; }

    public DateTime DataCriacao { get; protected set; }

    public Guid? CriadoPorId { get; protected set; }

    public DateTime? DataAlteracao { get; protected set; }

    public Guid? AlteradoPorId { get; protected set; }

    public bool Ativo { get; protected set; }

    public void Ativar(Guid usuario)
    {
        if (Ativo)
            return;

        Ativo = true;
        RegistrarAlteracao(usuario);
    }

    public void Desativar(Guid usuario)
    {
        if (!Ativo)
            return;

        Ativo = false;
        RegistrarAlteracao(usuario);
    }

    protected void RegistrarAlteracao(Guid usuarioId)
    {
        DataAlteracao = DateTime.UtcNow;
        AlteradoPorId = usuarioId;
    }
}
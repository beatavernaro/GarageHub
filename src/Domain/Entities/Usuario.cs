using Domain.Entities.Base;

namespace Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; }
    public string Email { get; private set; }
    public string SenhaHash { get; private set; }

    public Usuario(
        string nome,
        string email,
        string senhaHash,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;

        Normalizar();
    }

    public Usuario(
        Guid id,
        string nome,
        string email,
        string senhaHash,
        Guid? criadoPorId,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        Guid? alteradoPorId,
        bool ativo)
        : base(
            id,
            dataCriacao,
            criadoPorId,
            dataAlteracao,
            alteradoPorId,
            ativo)
    {
        Nome = nome;
        Email = email;
        SenhaHash = senhaHash;

        Normalizar();
    }

    public void Normalizar()
    {
        Nome = Nome.Trim();

        Email = Email
            .Trim()
            .ToLowerInvariant();
    }
}
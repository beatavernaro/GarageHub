using Domain.Entities.Base;

namespace Domain.Entities;

public class Usuario(string nome, string email, string senhaHash, Guid criadoPorId) : BaseEntity(criadoPorId)
{
    public string Nome { get; private set; } = nome;
    public string Email { get; private set; } = email;
    public string SenhaHash { get; private set; } = senhaHash;

    public void Normalizar()
    {
        Nome = Nome.Trim();

        Email = Email
            .Trim()
            .ToLowerInvariant();
    }
}
using Domain.Entities.Base;

namespace Domain.Entities;

public class Usuario : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string SenhaHash { get; private set; } = string.Empty;

    public void Normalizar()
    {
        Nome = Nome.Trim();

        Email = Email
            .Trim()
            .ToLowerInvariant();
    }
}
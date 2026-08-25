using Application.Interfaces.Security;

namespace Infrastructure.Security;

public class BCryptPasswordHasher
    : IPasswordHasher
{
    public bool Verificar(
        string senha,
        string senhaHash)
    {
        return BCrypt.Net.BCrypt.Verify(
            senha,
            senhaHash);
    }
}
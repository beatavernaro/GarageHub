using Domain.Entities;

namespace Application.Interfaces.Security;

public interface ITokenService
{
    string GerarToken(
        Usuario usuario,
        DateTime expiracao);
}
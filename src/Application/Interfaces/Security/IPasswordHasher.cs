namespace Application.Interfaces.Security;

public interface IPasswordHasher
{
    bool Verificar(
        string senha,
        string senhaHash);
}
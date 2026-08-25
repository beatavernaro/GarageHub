using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IUsuarioRepository
{
    Task<Usuario?> ObterPorEmailAsync(string email);
}
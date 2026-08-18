using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IClienteRepository
{
    Task<Cliente?> ObterPorIdAsync(Guid id);

    Task<Cliente?> ObterPorDocumentoAsync(string documento);

    Task<IEnumerable<Cliente>> ObterTodosAsync();

    Task AdicionarAsync(Cliente cliente);

    Task AtualizarAsync(Cliente cliente);
}
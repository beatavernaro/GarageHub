using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOrcamentoRepository
{
    Task<Orcamento?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<Orcamento>> ObterTodosAsync();

    Task<IEnumerable<Orcamento>> ObterPorClienteIdAsync(
        Guid clienteId);

    Task AdicionarAsync(Orcamento orcamento);

    Task AtualizarAsync(Orcamento orcamento);

    Task AtualizarItensAsync(Orcamento orcamento);
}
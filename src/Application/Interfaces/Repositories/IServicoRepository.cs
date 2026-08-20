using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IServicoRepository
{
    Task<Servico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<Servico>> ObterTodosAsync();

    Task<Servico?> ObterPorNomeAsync(string nome);

    Task AdicionarAsync(Servico servico);

    Task AtualizarAsync(Servico servico);

    Task AtualizarItensEstoqueAsync(Servico servico);
}
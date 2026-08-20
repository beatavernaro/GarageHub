using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IServicoRepository
{
    Task<Servico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<Servico>> ObterTodosAsync();

    Task<Servico?> ObterPorNomeAsync(string nome);

    Task AdicionarAsync(Servico servico);

    Task AtualizarAsync(Servico servico);

    Task AdicionarItemEstoqueAsync(
        ServicoItemEstoque item);

    Task AtualizarItemEstoqueAsync(
        ServicoItemEstoque item);

    Task InativarItemEstoqueAsync(
        Guid itemId,
        DateTime dataAlteracao,
        Guid usuarioId);
}
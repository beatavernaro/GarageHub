using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServico>> ObterTodosAsync();

    Task AdicionarAsync(OrdemServico ordemServico);

    Task AtualizarAsync(OrdemServico ordemServico);

    Task AdicionarItensAsync(OrdemServico ordemServico);

    Task AtualizarServicoStatusAsync(
        Guid ordemServicoId,
        Guid servicoId,
        Domain.Enums.StatusServico status,
        Guid usuarioId);
}
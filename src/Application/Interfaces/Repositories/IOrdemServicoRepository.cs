using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServico>> ObterTodosAsync();

    Task AdicionarAsync(OrdemServico ordemServico);

    Task AtualizarAsync(OrdemServico ordemServico);

    Task AtualizarServicoStatusAsync(
        Guid ordemServicoId,
        Guid ordemServicoServicoId,
        StatusServico status,
        DateTime dataAlteracao,
        Guid usuarioId);
}
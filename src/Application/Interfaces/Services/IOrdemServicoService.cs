using Application.DTOs.OrdemServico;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IOrdemServicoService
{
    Task<OrdemServicoDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync();

    Task<OrdemServicoDto> CriarAsync(
        Guid orcamentoId);

    Task IniciarAsync(Guid id);

    Task EntregarAsync(Guid id);

    Task AlterarStatusServicoAsync(
        Guid id,
        Guid servicoId,
        StatusServico status);
}
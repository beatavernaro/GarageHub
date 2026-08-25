using Application.DTOs.OrdemServico;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IOrdemServicoService
{
    Task<OrdemServicoDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync();

    Task<OrdemServicoDto> CriarAsync(Guid orcamentoId);

    Task AlterarStatusServicoAsync(
        Guid ordemServicoId,
        Guid ordemServicoServicoId,
        StatusServico status);

    Task EntregarAsync(Guid id);

    Task<TempoMedioOrdensServicoDto> ObterTempoMedioAsync();

    Task<AcompanhamentoOrdemServicoDto?> ObterAcompanhamentoAsync(string placa);
}
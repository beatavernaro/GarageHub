using Application.DTOs.OrdemServico;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IOrdemServicoService
{
    Task<OrdemServicoDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync();

    Task<StatusOrdemServicoDto?> ObterStatusAsync(Guid id);

    Task<IEnumerable<OrdemServicoDto>> ObterEmAndamentoAsync();

    Task<OrdemServicoDto> CriarAsync(Guid orcamentoId);

    Task<AberturaOrdemServicoDto> AbrirAsync(AbrirOrdemServicoDto dto);

    Task AlterarStatusServicoAsync(
        Guid ordemServicoId,
        Guid ordemServicoServicoId,
        StatusServico status);

    Task EntregarAsync(Guid id);

    Task<TempoMedioOrdensServicoDto> ObterTempoMedioAsync();

    Task<AcompanhamentoOrdemServicoDto?> ObterAcompanhamentoAsync(string placa);
}
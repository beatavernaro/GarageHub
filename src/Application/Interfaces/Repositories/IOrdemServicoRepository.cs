using Application.DTOs.OrdemServico;
using Domain.Entities;
using Domain.Enums;
namespace Application.Interfaces.Repositories;

public interface IOrdemServicoRepository
{
    Task<OrdemServico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrdemServico>> ObterTodosAsync();
    Task<OrdemServico?> ObterAtualPorPlacaAsync(string placa);

    Task AdicionarAsync(OrdemServico ordemServico);

    Task AtualizarAsync(OrdemServico ordemServico);

    Task AtualizarServicoStatusAsync(OrdemServicoServico servico);
    Task<IEnumerable<TempoOrdemServicoDto>> ObterTemposOrdensAsync();
}
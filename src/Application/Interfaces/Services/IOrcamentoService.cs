using Application.DTOs.Orcamento;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IOrcamentoService
{
    Task<OrcamentoDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<OrcamentoDto>> ObterTodosAsync();

    Task<IEnumerable<OrcamentoDto>> ObterPorClienteIdAsync(Guid clienteId);

    Task<OrcamentoDto> CriarAsync(CriarOrcamentoDto dto);

    Task AdicionarItemAsync(
        Guid id,
        AdicionarOrcamentoItemDto dto);

    Task RemoverItemAsync(
        Guid id,
        Guid itemId);

    Task AlterarQuantidadeItemAsync(
        Guid id,
        Guid itemId,
        int quantidade);

    Task AlterarValorUnitarioItemAsync(
        Guid id,
        Guid itemId,
        decimal valorUnitario);

    Task AplicarDescontoAsync(
        Guid id,
        decimal desconto);

    Task AlterarStatusAsync(
        Guid id,
        StatusOrcamento status);

    Task<ResultadoAprovacaoOrcamentoDto> AprovarAsync(Guid id);

    Task RejeitarAsync(Guid id);

    Task ColocarEmAguardandoClienteAsync(Guid id);

    Task CancelarAsync(Guid id);
}
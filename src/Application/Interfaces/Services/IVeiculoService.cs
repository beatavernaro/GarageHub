using Application.DTOs.Veiculo;

namespace Application.Interfaces.Services;

public interface IVeiculoService
{
    Task<VeiculoDto?> ObterPorIdAsync(Guid id);
    Task<IEnumerable<VeiculoDto>> ObterTodosAsync();
    Task<IEnumerable<VeiculoDto>> ObterPorClienteIdAsync(Guid clienteId);
    Task<VeiculoDto?> ObterPorPlacaAsync(string placa);

    Task<VeiculoDto> CriarAsync(CriarVeiculoDto dto);
    Task AtualizarAsync(Guid id, AtualizarVeiculoDto dto);
    Task InativarAsync(Guid id);
}
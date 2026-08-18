using Application.DTOs.Cliente;

namespace Application.Interfaces.Services;

public interface IClienteService
{
    Task<ClienteDto> ObterPorIdAsync(Guid id);

    Task<IEnumerable<ClienteDto>> ObterTodosAsync();

    Task<ClienteDto> ObterPorDocumentoAsync(string documento);
    Task<ClienteDto> CriarAsync(CriarClienteDto criarClienteDto);

    Task AtualizarAsync(Guid id, AtualizarClienteDto atualizarClienteDto);

    Task InativarAsync(Guid id);
}
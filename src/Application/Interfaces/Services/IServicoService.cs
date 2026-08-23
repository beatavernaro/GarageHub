using Application.DTOs.Servico;
using Domain.Enums;

namespace Application.Interfaces.Services;

public interface IServicoService
{
    Task<ServicoDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<ServicoDto>> ObterTodosAsync();

    Task<ServicoDto?> ObterPorNomeAsync(string nome);

    Task<ServicoDto> CriarAsync(CriarServicoDto dto);

    Task AtualizarAsync(
        Guid id,
        AtualizarServicoDto dto);

    Task AlterarPrecoAsync(
        Guid id,
        decimal novoPreco);

    Task InativarAsync(Guid id);

    Task AtivarAsync(Guid id);
}
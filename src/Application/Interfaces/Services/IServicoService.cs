using Application.DTOs.Servico;

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

    Task AdicionarItemEstoqueAsync(
        Guid id,
        AdicionarServicoItemEstoqueDto dto);

    Task RemoverItemEstoqueAsync(
        Guid id,
        Guid itemEstoqueId);

    Task AlterarQuantidadeItemEstoqueAsync(
        Guid id,
        Guid itemEstoqueId,
        int quantidade);

    Task AtualizarItensEstoqueAsync(
        Guid id);

    Task InativarAsync(Guid id);

    Task AtivarAsync(Guid id);
}
using Application.DTOs.ItemEstoque;

namespace Application.Interfaces.Services;

public interface IItemEstoqueService
{
    Task<ItemEstoqueDto?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<ItemEstoqueDto>> ObterTodosAsync();

    Task<ItemEstoqueDto?> ObterPorCodigoInternoAsync(string codigoInterno);

    Task<ItemEstoqueDto> CriarAsync(CriarItemEstoqueDto dto);

    Task AtualizarAsync(Guid id, AtualizarItemEstoqueDto dto);

    Task AdicionarEstoqueAsync(Guid id, int quantidade);

    Task RemoverEstoqueAsync(Guid id, int quantidade);

    Task AlterarPrecoAsync(Guid id, decimal novoPreco);

    Task InativarAsync(Guid id);
}
using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IItemEstoqueRepository
{
    Task<ItemEstoque?> ObterPorIdAsync(Guid id);

    Task<ItemEstoque?> ObterPorCodigoInternoAsync(string codigoInterno);

    Task<IEnumerable<ItemEstoque>> ObterTodosAsync();

    Task AdicionarAsync(ItemEstoque item);

    Task AtualizarAsync(ItemEstoque item);
}
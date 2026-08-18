using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IVeiculoRepository
{
    Task<Veiculo?> ObterPorIdAsync(Guid id);
    Task<Veiculo?> ObterPorPlacaAsync(string placa);
    Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(Guid clienteId);
    Task<IEnumerable<Veiculo>> ObterTodosAsync();
    Task AdicionarAsync(Veiculo veiculo);
    Task AtualizarAsync(Veiculo veiculo);
    Task DesativarAsync(Veiculo veiculo);
}

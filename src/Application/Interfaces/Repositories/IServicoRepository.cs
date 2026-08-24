using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface IServicoRepository
{
    Task<Servico?> ObterPorIdAsync(Guid id);

    Task<IEnumerable<Servico>> ObterTodosAsync();

    Task<Servico?> ObterPorCodigoInternoAsync(string codigoInterno);

    Task AdicionarAsync(Servico servico);

    Task AtualizarAsync(Servico servico);

}
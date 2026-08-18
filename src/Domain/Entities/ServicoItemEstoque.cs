using Domain.Entities.Base;
using Domain.Exceptions;

namespace Domain.Entities;

public class ServicoItemEstoque : BaseEntity
{
    public Guid ServicoId { get; private set; }
    public Guid ItemEstoqueId { get; private set; }
    public int Quantidade { get; private set; }

    public ServicoItemEstoque(
        Guid servicoId,
        Guid itemEstoqueId,
        int quantidade,
        Guid criadoPorId) : base(criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        Quantidade = quantidade;
    }

    public void AlterarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        Quantidade = quantidade;
    }
}
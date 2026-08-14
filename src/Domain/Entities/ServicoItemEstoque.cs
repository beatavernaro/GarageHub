using Domain.Entities.Base;

namespace Domain.Entities;

public class ServicoItemEstoque : BaseEntity
{
    public Guid ServicoId { get; private set; }
    public Guid ItemEstoqueId { get; private set; }
    public int Quantidade { get; private set; }
}
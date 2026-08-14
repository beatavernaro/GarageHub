using Domain.Entities.Base;

namespace Domain.Entities;

public class OrcamentoItem : BaseEntity
{
    public Guid OrcamentoId { get; private set; }

    public Guid? ServicoId { get; private set; }
    public Guid? ItemEstoqueId { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }
}
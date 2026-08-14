using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Orcamento : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }

    public StatusOrcamento Status { get; private set; }

    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }

    public DateTime? DataAprovacao { get; private set; }
    public DateTime? DataRejeicao { get; private set; }

    private readonly List<OrcamentoItem> _itens = [];
    public IReadOnlyCollection<OrcamentoItem> Itens => _itens;
}
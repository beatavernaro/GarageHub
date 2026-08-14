using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class OrdemServico : BaseEntity
{
    public Guid OrcamentoId { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }

    public StatusOrdemServico Status { get; private set; }

    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }

    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public DateTime? DataEntrega { get; private set; }

    private readonly List<OrdemServicoItem> _itens = [];
    public IReadOnlyCollection<OrdemServicoItem> Itens => _itens;

}
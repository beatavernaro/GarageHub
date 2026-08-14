using Domain.Entities.Base;

namespace Domain.Entities;

public class OrdemServicoItem : BaseEntity
{
    public Guid OrdemServicoId { get; private set; }
    public Guid ServicoId { get; private set; }

    public string NomeServico { get; private set; } = string.Empty;
    public string? DescricaoServico { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }
}
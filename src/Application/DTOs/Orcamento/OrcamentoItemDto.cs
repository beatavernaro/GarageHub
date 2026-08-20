namespace Application.DTOs.Orcamento;

public class OrcamentoItemDto
{
    public Guid Id { get; set; }

    public Guid? ServicoId { get; set; }

    public Guid? ItemEstoqueId { get; set; }

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }
}
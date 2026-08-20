namespace Application.DTOs.Orcamento;

public class AdicionarOrcamentoItemDto
{
    public Guid? ServicoId { get; set; }

    public Guid? ItemEstoqueId { get; set; }

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }
}
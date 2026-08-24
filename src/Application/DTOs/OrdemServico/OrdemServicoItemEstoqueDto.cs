namespace Application.DTOs.OrdemServico;

public class OrdemServicoItemEstoqueDto
{
    public Guid Id { get; set; }

    public Guid ItemEstoqueId { get; set; }

    public string NomeItem { get; set; } = string.Empty;

    public string? DescricaoItem { get; set; }

    public int Quantidade { get; set; }

    public decimal ValorUnitario { get; set; }

    public decimal ValorTotal { get; set; }
}
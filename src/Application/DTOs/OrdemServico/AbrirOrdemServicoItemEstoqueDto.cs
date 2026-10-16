namespace Application.DTOs.OrdemServico;

public class AbrirOrdemServicoItemEstoqueDto
{
    public Guid ItemEstoqueId { get; set; }

    public int Quantidade { get; set; }

    public decimal? ValorUnitario { get; set; }
}

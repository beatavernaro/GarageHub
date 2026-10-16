namespace Application.DTOs.OrdemServico;

public class AbrirOrdemServicoServicoDto
{
    public Guid ServicoId { get; set; }

    public int Quantidade { get; set; }

    public decimal? ValorUnitario { get; set; }
}

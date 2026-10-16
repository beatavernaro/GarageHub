namespace Application.DTOs.OrdemServico;

public class AbrirOrdemServicoDto
{
    public Guid ClienteId { get; set; }

    public Guid VeiculoId { get; set; }

    public decimal Desconto { get; set; }

    public List<AbrirOrdemServicoServicoDto> Servicos { get; set; } = [];

    public List<AbrirOrdemServicoItemEstoqueDto> Itens { get; set; } = [];
}

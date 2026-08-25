namespace Application.DTOs.OrdemServico;

public class TempoMedioOrdensServicoDto
{
    public int QuantidadeOrdens { get; set; }

    public string TempoMedioGeral { get; set; } = string.Empty;

    public List<TempoOrdemServicoDto> Ordens { get; set; } = [];
}
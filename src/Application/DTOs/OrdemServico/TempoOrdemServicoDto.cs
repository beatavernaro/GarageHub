namespace Application.DTOs.OrdemServico;

public class TempoOrdemServicoDto
{
    public Guid OrdemServicoId { get; set; }

    public DateTime DataInicio { get; set; }

    public DateTime DataFinalizacao { get; set; }

    public string TempoExecucao { get; set; } = string.Empty;
}
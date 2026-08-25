namespace Application.DTOs.Servico;

public class TempoMedioServicoDto
{
    public Guid ServicoId { get; set; }

    public string CodigoInterno { get; set; } = string.Empty;

    public string NomeServico { get; set; } = string.Empty;

    public int QuantidadeExecucoes { get; set; }

    public double TempoMedioSegundos { get; set; }

    public string TempoMedio { get; set; } = string.Empty;
}
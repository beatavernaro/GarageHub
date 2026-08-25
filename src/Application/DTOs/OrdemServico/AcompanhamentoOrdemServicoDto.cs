namespace Application.DTOs.OrdemServico;

public class AcompanhamentoOrdemServicoDto
{
    public string Cliente { get; set; } = string.Empty;

    public string Veiculo { get; set; } = string.Empty;

    public string Placa { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public DateTime? DataInicio { get; set; }

    public List<AcompanhamentoServicoDto> Servicos { get; set; } = [];
}
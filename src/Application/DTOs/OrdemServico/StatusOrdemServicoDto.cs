using Domain.Enums;

namespace Application.DTOs.OrdemServico;

public class StatusOrdemServicoDto
{
    public Guid OrdemServicoId { get; set; }

    public StatusOrdemServico Status { get; set; }

    public string Descricao { get; set; } = string.Empty;
}

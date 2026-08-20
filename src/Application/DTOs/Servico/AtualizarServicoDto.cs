using Domain.Enums;

namespace Application.DTOs.Servico;

public class AtualizarServicoDto
{
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public StatusServico Status { get; set; }
}
using Domain.Enums;

namespace Application.DTOs.OrdemServico;

public class OrdemServicoServicoDto
{
    public Guid Id { get; set; }
    public Guid ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public string? DescricaoServico { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusServico Status { get; set; }
}
using Domain.Enums;

namespace Application.DTOs.OrdemServico;

public class OrdemServicoDto
{
    public Guid Id { get; set; }

    public Guid OrcamentoId { get; set; }

    public Guid ClienteId { get; set; }

    public Guid VeiculoId { get; set; }

    public StatusOrdemServico Status { get; set; }

    public decimal Desconto { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime? DataInicio { get; set; }

    public DateTime? DataFinalizacao { get; set; }

    public DateTime? DataEntrega { get; set; }

    public bool Ativo { get; set; }

    public List<OrdemServicoServicoDto> Servicos { get; set; } = [];

    public List<OrdemServicoItemEstoqueDto> Itens { get; set; } = [];
}
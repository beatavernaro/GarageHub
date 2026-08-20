using Domain.Enums;

namespace Application.DTOs.Orcamento;

public class OrcamentoDto
{
    public Guid Id { get; set; }

    public Guid ClienteId { get; set; }

    public Guid VeiculoId { get; set; }

    public StatusOrcamento Status { get; set; }

    public decimal Desconto { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime? DataEnvioCliente { get; set; }

    public DateTime? DataAprovacao { get; set; }

    public DateTime? DataRejeicao { get; set; }

    public bool Ativo { get; set; }

    public List<OrcamentoItemDto> Itens { get; set; } = [];
}
using Domain.Enums;

namespace Infrastructure.Mappers;

public class OrcamentoDbModel
{
    public Guid Id { get; init; }

    public Guid ClienteId { get; init; }

    public Guid VeiculoId { get; init; }

    public StatusOrcamento Status { get; init; }

    public decimal Desconto { get; init; }

    public decimal ValorTotal { get; init; }

    public DateTime? DataEnvioCliente { get; init; }

    public DateTime? DataAprovacao { get; init; }

    public DateTime? DataRejeicao { get; init; }

    public Guid? CriadoPorId { get; init; }

    public DateTime DataCriacao { get; init; }

    public DateTime? DataAlteracao { get; init; }

    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }

    public List<OrcamentoItemDbModel> Itens { get; set; } = [];
}
using Domain.Enums;

namespace Infrastructure.Mappers;

public class OrdemServicoDbModel
{
    public Guid Id { get; init; }

    public Guid OrcamentoId { get; init; }

    public Guid ClienteId { get; init; }

    public Guid VeiculoId { get; init; }

    public StatusOrdemServico Status { get; init; }

    public decimal Desconto { get; init; }

    public decimal ValorTotal { get; init; }

    public DateTime? DataInicio { get; init; }

    public DateTime? DataFinalizacao { get; init; }

    public DateTime? DataEntrega { get; init; }

    public Guid? CriadoPorId { get; init; }

    public DateTime DataCriacao { get; init; }

    public DateTime? DataAlteracao { get; init; }

    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }

    public List<OrdemServicoServicoDbModel> Servicos { get; set; } = [];

    public List<OrdemServicoItemEstoqueDbModel> Itens { get; set; } = [];
}
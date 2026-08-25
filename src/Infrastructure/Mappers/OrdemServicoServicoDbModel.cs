using Domain.Enums;

namespace Infrastructure.Mappers;

public class OrdemServicoServicoDbModel
{
    public Guid Id { get; init; }

    public Guid OrdemServicoId { get; init; }

    public Guid ServicoId { get; init; }

    public string NomeServico { get; init; } = string.Empty;

    public string? DescricaoServico { get; init; }

    public int Quantidade { get; init; }

    public decimal ValorUnitario { get; init; }

    public decimal ValorTotal { get; init; }

    public StatusServico Status { get; init; }

    public DateTime? DataInicio { get; init; }

    public DateTime? DataFinalizacao { get; init; }

    public Guid? CriadoPorId { get; init; }

    public DateTime DataCriacao { get; init; }

    public DateTime? DataAlteracao { get; init; }

    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }
}
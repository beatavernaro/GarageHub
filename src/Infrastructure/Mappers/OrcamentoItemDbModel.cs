namespace Infrastructure.Mappers;

public class OrcamentoItemDbModel
{
    public Guid Id { get; init; }
    public Guid OrcamentoId { get; init; }
    public Guid? ServicoId { get; init; }
    public Guid? ItemEstoqueId { get; init; }

    public string NomeItem { get; init; } = string.Empty;
    public string? DescricaoItem { get; init; }

    public int Quantidade { get; init; }
    public decimal ValorUnitario { get; init; }
    public decimal ValorTotal { get; init; }

    public Guid? CriadoPorId { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime? DataAlteracao { get; init; }
    public Guid? AlteradoPorId { get; init; }
    public bool Ativo { get; init; }
}
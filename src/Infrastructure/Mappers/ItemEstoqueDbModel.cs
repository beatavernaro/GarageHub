using Domain.Enums;

namespace Infrastructure.Mappers;

public class ItemEstoqueDbModel
{
    public Guid Id { get; init; }

    public string CodigoInterno { get; init; } = string.Empty;

    public string Nome { get; init; } = string.Empty;

    public string? Descricao { get; init; }

    public TipoItemEstoque Tipo { get; init; }

    public decimal Preco { get; init; }

    public int Estoque { get; init; }

    public Guid? CriadoPorId { get; init; }

    public DateTime DataCriacao { get; init; }

    public DateTime? DataAlteracao { get; init; }

    public Guid? AlteradoPorId { get; init; }

    public bool Ativo { get; init; }
}
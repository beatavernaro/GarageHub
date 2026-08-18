using Domain.Enums;

namespace Infrastructure.Mappers;

public class ClienteDbModel
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = string.Empty;
    public string Documento { get; init; } = string.Empty;
    public TipoPessoa TipoPessoa { get; init; }
    public string Telefone { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Logradouro { get; init; }
    public string? Numero { get; init; }
    public string? Complemento { get; init; }
    public string? Bairro { get; init; }
    public string? Cidade { get; init; }
    public string? Estado { get; init; }
    public string? Cep { get; init; }
    public Guid? CriadoPorId { get; init; }
    public DateTime DataCriacao { get; init; }
    public DateTime? DataAlteracao { get; init; }
    public Guid? AlteradoPorId { get; init; }
    public bool Ativo { get; init; }
}
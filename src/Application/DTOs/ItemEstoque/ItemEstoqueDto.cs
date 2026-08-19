using Domain.Enums;

namespace Application.DTOs.ItemEstoque;

public class ItemEstoqueDto
{
    public Guid Id { get; set; }
    public string CodigoInterno { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public TipoItemEstoque Tipo { get; set; }
    public decimal Preco { get; set; }
    public int Estoque { get; set; }
    public bool Ativo { get; set; }
}
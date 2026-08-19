using Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.ItemEstoque;

public class AtualizarItemEstoqueDto
{
    [Required]
    [StringLength(7, MinimumLength = 7)]
    public string CodigoInterno { get; set; } = string.Empty;

    [Required]
    [StringLength(150)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required]
    public TipoItemEstoque Tipo { get; set; }

    [Range(0.01, double.MaxValue)]
    public decimal Preco { get; set; }
}
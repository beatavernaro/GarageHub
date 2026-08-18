using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Application.DTOs.Cliente;

public class CriarClienteDto
{
    [Required]
    [StringLength(150, MinimumLength = 3)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [StringLength(14, MinimumLength = 11)]
    public string Documento { get; set; } = string.Empty;

    [Required]
    public TipoPessoa TipoPessoa { get; set; }

    [Required]
    [StringLength(11, MinimumLength = 10)]
    public string Telefone { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [StringLength(150)]
    public string Email { get; set; } = string.Empty;

    public EnderecoDto? Endereco { get; set; }
}
using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.Cliente;

public class EnderecoDto
{
    [Required]
    [StringLength(200)]
    public string Logradouro { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Numero { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Complemento { get; set; }

    [Required]
    [StringLength(100)]
    public string Bairro { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Cidade { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string Estado { get; set; } = string.Empty;

    [Required]
    [StringLength(8, MinimumLength = 8)]
    public string Cep { get; set; } = string.Empty;
}
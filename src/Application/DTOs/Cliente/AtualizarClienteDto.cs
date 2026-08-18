using Domain.Enums;

namespace Application.DTOs.Cliente;

public class AtualizarClienteDto
{
    public string Nome { get; set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; set; }
    public string Telefone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public EnderecoDto? Endereco { get; set; }
}
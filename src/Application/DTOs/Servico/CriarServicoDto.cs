using Domain.Enums;

namespace Application.DTOs.Servico;

public class CriarServicoDto
{
    public string CodigoInterno { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

}
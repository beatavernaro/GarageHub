using Domain.Enums;

namespace Application.DTOs.Servico;

public class ServicoDto
{
    public Guid Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public string? Descricao { get; set; }

    public decimal Preco { get; set; }

    public StatusServico Status { get; set; }

    public bool Ativo { get; set; }

    public List<ServicoItemEstoqueDto> ItensEstoque { get; set; } = [];
}
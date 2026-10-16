using Application.DTOs.Orcamento;

namespace Application.DTOs.OrdemServico;

public class AberturaOrdemServicoDto
{
    public Guid OrdemServicoId { get; set; }

    public Guid OrcamentoId { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public List<ItemEstoqueInsuficienteDto> ItensInsuficientes { get; set; } = [];

    public bool PossuiEstoqueInsuficiente =>
        ItensInsuficientes.Count > 0;
}

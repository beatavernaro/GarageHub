namespace Application.DTOs.Orcamento;

public class ResultadoAprovacaoOrcamentoDto
{
    public Guid OrcamentoId { get; set; }

    public string Mensagem { get; set; } = string.Empty;

    public List<ItemEstoqueInsuficienteDto> ItensInsuficientes { get; set; } = [];

    public bool PossuiEstoqueInsuficiente =>
        ItensInsuficientes.Count > 0;
}
namespace Application.DTOs.Orcamento;

public class ItemEstoqueInsuficienteDto
{
    public Guid ItemEstoqueId { get; set; }

    public string Nome { get; set; } = string.Empty;

    public int QuantidadeDisponivel { get; set; }

    public int QuantidadeNecessaria { get; set; }

    public int QuantidadeFaltante { get; set; }
}
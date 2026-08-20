namespace Application.DTOs.Servico;

public class ServicoItemEstoqueDto
{
    public Guid Id { get; set; }

    public Guid ItemEstoqueId { get; set; }

    public int Quantidade { get; set; }

    public bool Ativo { get; set; }
}
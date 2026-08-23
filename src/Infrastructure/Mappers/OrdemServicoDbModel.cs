using Domain.Enums;

namespace Infrastructure.Models;

public class OrdemServicoDbModel
{
    public Guid Id { get; set; }
    public Guid OrcamentoId { get; set; }
    public Guid ClienteId { get; set; }
    public Guid VeiculoId { get; set; }
    public int Status { get; set; }
    public decimal Desconto { get; set; }
    public decimal ValorTotal { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFinalizacao { get; set; }
    public DateTime? DataEntrega { get; set; }
    public Guid? CriadoPorId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }
    public bool Ativo { get; set; }

    public List<OrdemServicoItemEstoqueDbModel> Itens { get; set; } = [];
    public List<OrdemServicoServicoDbModel> Servicos { get; set; } = [];
}

public class OrdemServicoItemEstoqueDbModel
{
    public Guid Id { get; set; }
    public Guid OrdemServicoId { get; set; }
    public Guid ItemEstoqueId { get; set; }
    public string NomeItem { get; set; } = string.Empty;
    public string? DescricaoItem { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public Guid? CriadoPorId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }
    public bool Ativo { get; set; }
}

public class OrdemServicoServicoDbModel
{
    public Guid Id { get; set; }
    public Guid OrdemServicoId { get; set; }
    public Guid ServicoId { get; set; }
    public string NomeServico { get; set; } = string.Empty;
    public string? DescricaoServico { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal ValorTotal { get; set; }
    public int Status { get; set; }
    public Guid? CriadoPorId { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAlteracao { get; set; }
    public Guid? AlteradoPorId { get; set; }
    public bool Ativo { get; set; }
}
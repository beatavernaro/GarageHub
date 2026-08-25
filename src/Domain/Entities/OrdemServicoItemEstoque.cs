using Domain.Entities.Base;
using Domain.Exceptions;

namespace Domain.Entities;

public class OrdemServicoItemEstoque : BaseEntity
{
    public Guid OrdemServicoId { get; private set; }
    public Guid ItemEstoqueId { get; private set; }

    public string NomeItem { get; private set; } = string.Empty;
    public string? DescricaoItem { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Construtor utilizado na criação da entidade
    public OrdemServicoItemEstoque(
        Guid ordemServicoId,
        Guid itemEstoqueId,
        string nomeItem,
        string? descricaoItem,
        int quantidade,
        decimal valorUnitario,
        Guid? criadoPorId)
        : base(criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        if (valorUnitario <= 0)
            throw new DomainException("O valor unitário deve ser maior que zero.");

        OrdemServicoId = ordemServicoId;
        ItemEstoqueId = itemEstoqueId;
        NomeItem = nomeItem;
        DescricaoItem = descricaoItem;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = quantidade * valorUnitario;
    }

    // Construtor utilizado pelo mapper ao carregar do banco
    public OrdemServicoItemEstoque(
        Guid id,
        Guid ordemServicoId,
        Guid itemEstoqueId,
        string nomeItem,
        string? descricaoItem,
        int quantidade,
        decimal valorUnitario,
        decimal valorTotal,
        Guid? criadoPorId,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        Guid? alteradoPorId,
        bool ativo)
        : base(
            id,
            dataCriacao,
            criadoPorId,
            dataAlteracao,
            alteradoPorId,
            ativo)
    {
        OrdemServicoId = ordemServicoId;
        ItemEstoqueId = itemEstoqueId;
        NomeItem = nomeItem;
        DescricaoItem = descricaoItem;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = valorTotal;
    }
}
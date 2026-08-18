using Domain.Entities.Base;
using Domain.Exceptions;

namespace Domain.Entities;

public class OrdemServicoItem : BaseEntity
{
    public Guid OrdemServicoId { get; private set; }
    public Guid ServicoId { get; private set; }

    public string NomeServico { get; private set; } = string.Empty;
    public string? DescricaoServico { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    public OrdemServicoItem(
        Guid ordemServicoId,
        Guid servicoId,
        string nomeServico,
        string? descricaoServico,
        int quantidade,
        decimal valorUnitario,
        Guid criadoPorId) : base(criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        if (valorUnitario <= 0)
            throw new DomainException("O valor unitário deve ser maior que zero.");

        OrdemServicoId = ordemServicoId;
        ServicoId = servicoId;
        NomeServico = nomeServico;
        DescricaoServico = descricaoServico;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = quantidade * valorUnitario;
    }
}
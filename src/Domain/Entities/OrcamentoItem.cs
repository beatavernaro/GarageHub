using Domain.Entities.Base;
using Domain.Exceptions;

namespace Domain.Entities;

public class OrcamentoItem : BaseEntity
{
    public Guid OrcamentoId { get; private set; }
    public Guid? ServicoId { get; private set; }
    public Guid? ItemEstoqueId { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    public OrcamentoItem(
        Guid orcamentoId,
        Guid? servicoId,
        Guid? itemEstoqueId,
        int quantidade,
        decimal valorUnitario,
        Guid criadoPorId) : base(criadoPorId)
    {
        if (servicoId.HasValue == itemEstoqueId.HasValue)
            throw new DomainException(
                "O item deve ser um serviço ou um item de estoque.");

        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        if (valorUnitario <= 0)
            throw new DomainException(
                "O valor unitário deve ser maior que zero.");

        OrcamentoId = orcamentoId;
        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = quantidade * valorUnitario;
    }

    public void AlterarQuantidade(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        Quantidade = quantidade;
        RecalcularTotal();
    }

    public void AlterarValorUnitario(decimal valorUnitario)
    {
        if (valorUnitario <= 0)
            throw new DomainException(
                "O valor unitário deve ser maior que zero.");

        ValorUnitario = valorUnitario;
        RecalcularTotal();
    }

    private void RecalcularTotal()
    {
        ValorTotal = Quantidade * ValorUnitario;
    }
}
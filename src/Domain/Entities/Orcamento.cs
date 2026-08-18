using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Orcamento(Guid clienteId, Guid veiculoId, Guid criadoPorId) : BaseEntity(criadoPorId)
{
    public Guid ClienteId { get; private set; } = clienteId;
    public Guid VeiculoId { get; private set; } = veiculoId;

    public StatusOrcamento Status { get; private set; } = StatusOrcamento.EmElaboracao;

    public decimal Desconto { get; private set; } = 0;
    public decimal ValorTotal { get; private set; } = 0;
    public DateTime? DataAprovacao { get; private set; }
    public DateTime? DataRejeicao { get; private set; }
    private readonly List<OrcamentoItem> _itens = [];
    public IReadOnlyCollection<OrcamentoItem> Itens => _itens;

    public void AdicionarItem(OrcamentoItem item)
    {
        if (Status != StatusOrcamento.EmElaboracao)
            throw new DomainException(
                "Não é possível adicionar itens a um orçamento que não está em elaboração.");

        _itens.Add(item);

        CalcularTotal();
    }

    public void RemoverItem(Guid itemId)
    {
        if (Status != StatusOrcamento.EmElaboracao)
            throw new DomainException(
                "Não é possível remover itens de um orçamento que não está em elaboração.");

        var item = _itens.FirstOrDefault(x => x.Id == itemId) ?? throw new DomainException(
                "Item não encontrado no orçamento.");
        _itens.Remove(item);

        CalcularTotal();
    }

    public void AplicarDesconto(decimal desconto)
    {
        if (Status != StatusOrcamento.EmElaboracao)
            throw new DomainException(
                "Não é possível alterar o desconto de um orçamento que não está em elaboração.");

        if (desconto < 0)
            throw new DomainException(
                "O desconto não pode ser negativo.");

        var subtotal = _itens.Sum(x => x.ValorTotal);

        if (desconto > subtotal)
            throw new DomainException(
                "O desconto não pode ser maior que o valor do orçamento.");

        Desconto = desconto;

        CalcularTotal();
    }

    public void Aprovar()
    {
        if (Status != StatusOrcamento.AguardandoCliente)
            throw new DomainException(
                "O orçamento não está aguardando aprovação do cliente.");

        Status = StatusOrcamento.Aprovado;
        DataAprovacao = DateTime.UtcNow;
    }

    public void Rejeitar()
    {
        if (Status != StatusOrcamento.AguardandoCliente)
            throw new DomainException(
                "O orçamento não está aguardando aprovação do cliente.");

        Status = StatusOrcamento.Rejeitado;
        DataRejeicao = DateTime.UtcNow;
    }

    public void ColocarEmAguardandoCliente()
    {
        if (Status != StatusOrcamento.EmElaboracao)
            throw new DomainException(
                "O orçamento não está em elaboração.");

        if (_itens.Count == 0)
            throw new DomainException(
                "O orçamento deve possuir pelo menos um item.");

        Status = StatusOrcamento.AguardandoCliente;
    }

    private void CalcularTotal()
    {
        var subtotal = _itens.Sum(x => x.ValorTotal);
        ValorTotal = subtotal - Desconto;
    }
}
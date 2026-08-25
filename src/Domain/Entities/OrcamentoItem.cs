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
    public string NomeItem { get; private set; } = string.Empty;
    public string? DescricaoItem { get; private set; }

    public void AlterarQuantidade(int quantidade, Guid? usuarioId)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        Quantidade = quantidade;

        RecalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    public void AlterarValorUnitario(decimal valorUnitario, Guid? usuarioId)
    {
        if (valorUnitario <= 0)
            throw new DomainException("O valor unitário deve ser maior que zero.");

        ValorUnitario = valorUnitario;

        RecalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    private static void ValidarTipoItem(Guid? servicoId, Guid? itemEstoqueId)
    {
        if (servicoId.HasValue == itemEstoqueId.HasValue)
            throw new DomainException("O item deve ser um serviço ou um item de estoque.");
    }

    private void RecalcularTotal()
    {
        ValorTotal = Quantidade * ValorUnitario;
    }

    public OrcamentoItem(Guid orcamentoId, Guid? servicoId, Guid? itemEstoqueId, string nomeItem, string? descricaoItem, int quantidade, decimal valorUnitario, Guid? criadoPorId)
    : base(criadoPorId)
    {
        ValidarTipoItem(servicoId, itemEstoqueId);

        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");

        if (valorUnitario <= 0)
            throw new DomainException("O valor unitário deve ser maior que zero.");

        OrcamentoId = orcamentoId;
        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        NomeItem = nomeItem;
        DescricaoItem = descricaoItem;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;

        RecalcularTotal();
    }

    public OrcamentoItem(Guid id, Guid orcamentoId, Guid? servicoId, Guid? itemEstoqueId, string nomeItem, string? descricaoItem, int quantidade, decimal valorUnitario, decimal valorTotal, Guid? criadoPorId, DateTime dataCriacao, DateTime? dataAlteracao, Guid? alteradoPorId, bool ativo)
    : base(id, dataCriacao, criadoPorId, dataAlteracao, alteradoPorId, ativo)
    {
        ValidarTipoItem(servicoId, itemEstoqueId);

        OrcamentoId = orcamentoId;
        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        NomeItem = nomeItem;
        DescricaoItem = descricaoItem;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = valorTotal;
    }
}
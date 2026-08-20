using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Orcamento : BaseEntity
{
    private readonly List<OrcamentoItem> _itens = [];

    public Orcamento(
        Guid clienteId,
        Guid veiculoId,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Status = StatusOrcamento.EmElaboracao;
        Desconto = 0;
        ValorTotal = 0;
    }

    public Orcamento(
        Guid id,
        Guid clienteId,
        Guid veiculoId,
        StatusOrcamento status,
        decimal desconto,
        decimal valorTotal,
        DateTime? dataEnvioCliente,
        DateTime? dataAprovacao,
        DateTime? dataRejeicao,
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
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Status = status;
        Desconto = desconto;
        ValorTotal = valorTotal;
        DataEnvioCliente = dataEnvioCliente;
        DataAprovacao = dataAprovacao;
        DataRejeicao = dataRejeicao;
    }

    public Guid ClienteId { get; private set; }

    public Guid VeiculoId { get; private set; }

    public StatusOrcamento Status { get; private set; }

    public decimal Desconto { get; private set; }

    public decimal ValorTotal { get; private set; }

    public DateTime? DataEnvioCliente { get; private set; }

    public DateTime? DataAprovacao { get; private set; }

    public DateTime? DataRejeicao { get; private set; }

    public IReadOnlyCollection<OrcamentoItem> Itens => _itens;

    public void AdicionarItem(OrcamentoItem item, Guid usuarioId)
    {
        ValidarEmElaboracao();

        _itens.Add(item);

        CalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    public void RemoverItem(
    Guid itemId,
    Guid usuarioId)
    {
        if (Status != StatusOrcamento.EmElaboracao)
            throw new DomainException(
                "Não é possível remover itens de um orçamento que não está em elaboração.");

        var item = _itens.FirstOrDefault(
            x => x.Id == itemId && x.Ativo)
            ?? throw new DomainException(
                "Item não encontrado no orçamento.");

        item.Desativar(usuarioId);

        CalcularTotal();
    }

    public void AlterarQuantidadeItem(
        Guid itemId,
        int quantidade,
        Guid usuarioId)
    {
        ValidarEmElaboracao();

        var item = _itens.FirstOrDefault(x => x.Id == itemId && x.Ativo)
            ?? throw new DomainException(
                "Item não encontrado no orçamento.");

        item.AlterarQuantidade(quantidade, usuarioId);

        CalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    public void AlterarValorUnitarioItem(
        Guid itemId,
        decimal valorUnitario,
        Guid usuarioId)
    {
        ValidarEmElaboracao();

        var item = _itens.FirstOrDefault(x => x.Id == itemId && x.Ativo)
            ?? throw new DomainException(
                "Item não encontrado no orçamento.");

        item.AlterarValorUnitario(valorUnitario, usuarioId);

        CalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    public void AplicarDesconto(
        decimal desconto,
        Guid usuarioId)
    {
        ValidarEmElaboracao();

        if (desconto < 0)
            throw new DomainException(
                "O desconto não pode ser negativo.");

        var subtotal = _itens
            .Where(x => x.Ativo)
            .Sum(x => x.ValorTotal);
            
        if (desconto > subtotal)
            throw new DomainException(
                "O desconto não pode ser maior que o valor do orçamento.");

        Desconto = desconto;

        CalcularTotal();
        RegistrarAlteracao(usuarioId);
    }

    public void ColocarEmAguardandoCliente(Guid usuarioId)
    {
        ValidarStatus(StatusOrcamento.EmElaboracao);

        if (!_itens.Any(x => x.Ativo))
            throw new DomainException(
                "O orçamento deve possuir pelo menos um item.");

        Status = StatusOrcamento.AguardandoCliente;
        DataEnvioCliente = DateTime.UtcNow;

        RegistrarAlteracao(usuarioId);
    }

    public void Aprovar(Guid usuarioId)
    {
        ValidarStatus(StatusOrcamento.AguardandoCliente);

        Status = StatusOrcamento.Aprovado;
        DataAprovacao = DateTime.UtcNow;

        RegistrarAlteracao(usuarioId);
    }

    public void Rejeitar(Guid usuarioId)
    {
        ValidarStatus(StatusOrcamento.AguardandoCliente);

        Status = StatusOrcamento.Rejeitado;
        DataRejeicao = DateTime.UtcNow;

        RegistrarAlteracao(usuarioId);
    }

    public void Cancelar(Guid usuarioId)
    {
        if (Status is StatusOrcamento.Aprovado
            or StatusOrcamento.Rejeitado
            or StatusOrcamento.Cancelado
            or StatusOrcamento.Expirado)
        {
            throw new DomainException(
                "Não é possível cancelar um orçamento finalizado.");
        }

        Status = StatusOrcamento.Cancelado;

        RegistrarAlteracao(usuarioId);
    }

    public void Expirar(Guid usuarioId)
    {
        ValidarStatus(StatusOrcamento.AguardandoCliente);

        if (!DataEnvioCliente.HasValue)
            throw new DomainException(
                "Não foi possível determinar a data de envio ao cliente.");

        if (DataEnvioCliente.Value.AddDays(15) > DateTime.UtcNow)
            throw new DomainException(
                "O orçamento ainda não atingiu o prazo para expiração.");

        Status = StatusOrcamento.Expirado;

        RegistrarAlteracao(usuarioId);
    }

    public void VerificarExpiracao(Guid usuarioId)
    {
        if (Status != StatusOrcamento.AguardandoCliente)
            return;

        if (!DataEnvioCliente.HasValue)
            return;

        if (DataEnvioCliente.Value.AddDays(15) <= DateTime.UtcNow)
            Expirar(usuarioId);
    }

    public void CarregarItens(IEnumerable<OrcamentoItem> itens)
    {
        _itens.Clear();
        _itens.AddRange(itens);

        CalcularTotal();
    }

    private void ValidarEmElaboracao()
    {
        ValidarStatus(StatusOrcamento.EmElaboracao);
    }

    private void ValidarStatus(StatusOrcamento statusEsperado)
    {
        if (Status != statusEsperado)
        {
            throw new DomainException(
                "Não é possível realizar esta operação no status atual do orçamento.");
        }
    }

    private void CalcularTotal()
    {
        var subtotal = _itens
            .Where(x => x.Ativo)
            .Sum(x => x.ValorTotal);

        ValorTotal = subtotal - Desconto;
    }
}
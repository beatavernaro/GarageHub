using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class OrdemServico : BaseEntity
{
    public Guid OrcamentoId { get; private set; }
    public Guid ClienteId { get; private set; }
    public Guid VeiculoId { get; private set; }

    public StatusOrdemServico Status { get; private set; }

    public decimal Desconto { get; private set; }
    public decimal ValorTotal { get; private set; }

    public DateTime? DataInicio { get; private set; }
    public DateTime? DataFinalizacao { get; private set; }
    public DateTime? DataEntrega { get; private set; }

    private readonly List<OrdemServicoItem> _itens = [];
    public IReadOnlyCollection<OrdemServicoItem> Itens => _itens;

    public OrdemServico(Guid orcamentoId, Guid clienteId, Guid veiculoId, decimal desconto, decimal valorTotal, IEnumerable<OrdemServicoItem> itens, Guid criadoPorId) : base(criadoPorId)
    {
        OrcamentoId = orcamentoId;
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Desconto = desconto;
        ValorTotal = valorTotal;
        Status = StatusOrdemServico.AguardandoExecucao;

        _itens.AddRange(itens);
    }

    public void Iniciar()
    {
        if (Status != StatusOrdemServico.AguardandoExecucao)
            throw new DomainException(
                "A ordem de serviço não está aguardando início.");

        Status = StatusOrdemServico.EmExecucao;
        DataInicio = DateTime.UtcNow;
    }

    public void Finalizar()
    {
        if (Status != StatusOrdemServico.EmExecucao)
            throw new DomainException(
                "A ordem de serviço não está em execução.");

        Status = StatusOrdemServico.Finalizada;
        DataFinalizacao = DateTime.UtcNow;
    }

    public void Entregar()
    {
        if (Status != StatusOrdemServico.Finalizada)
            throw new DomainException(
                "A ordem de serviço não está finalizada.");

        Status = StatusOrdemServico.Entregue;
        DataEntrega = DateTime.UtcNow;
    }
}
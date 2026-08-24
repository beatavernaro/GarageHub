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

    private readonly List<OrdemServicoItemEstoque> _itens = [];
    private readonly List<OrdemServicoServico> _servicos = [];

    public IReadOnlyCollection<OrdemServicoItemEstoque> Itens => _itens;
    public IReadOnlyCollection<OrdemServicoServico> Servicos => _servicos;

    // Construtor utilizado na criação da entidade
    public OrdemServico(
    Guid id,
    Guid orcamentoId,
    Guid clienteId,
    Guid veiculoId,
    decimal desconto,
    decimal valorTotal,
    IEnumerable<OrdemServicoItemEstoque> itens,
    IEnumerable<OrdemServicoServico> servicos,
    Guid criadoPorId)
    : base(
        id,
        DateTime.UtcNow,
        criadoPorId,
        null,
        null,
        true)
{
    OrcamentoId = orcamentoId;
    ClienteId = clienteId;
    VeiculoId = veiculoId;
    Desconto = desconto;
    ValorTotal = valorTotal;
    Status = StatusOrdemServico.AguardandoExecucao;

    _itens.AddRange(itens);
    _servicos.AddRange(servicos);
}

    // Construtor utilizado pelo mapper ao carregar do banco
    public OrdemServico(
        Guid id,
        Guid orcamentoId,
        Guid clienteId,
        Guid veiculoId,
        StatusOrdemServico status,
        decimal desconto,
        decimal valorTotal,
        DateTime? dataInicio,
        DateTime? dataFinalizacao,
        DateTime? dataEntrega,
        Guid? criadoPorId,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        Guid? alteradoPorId,
        bool ativo,
        IEnumerable<OrdemServicoItemEstoque>? itens = null,
        IEnumerable<OrdemServicoServico>? servicos = null)
        : base(
            id,
            dataCriacao,
            criadoPorId,
            dataAlteracao,
            alteradoPorId,
            ativo)
    {
        OrcamentoId = orcamentoId;
        ClienteId = clienteId;
        VeiculoId = veiculoId;
        Status = status;
        Desconto = desconto;
        ValorTotal = valorTotal;
        DataInicio = dataInicio;
        DataFinalizacao = dataFinalizacao;
        DataEntrega = dataEntrega;

        if (itens is not null)
            _itens.AddRange(itens);

        if (servicos is not null)
            _servicos.AddRange(servicos);
    }

    public void Iniciar(Guid usuarioId)
    {
        if (Status != StatusOrdemServico.AguardandoExecucao)
            throw new DomainException(
                "A ordem de serviço não está aguardando início.");

        Status = StatusOrdemServico.EmExecucao;
        DataInicio = DateTime.UtcNow;

        RegistrarAlteracao(usuarioId);
    }

    public void AtualizarStatus(Guid usuarioId)
    {
        if (Status is StatusOrdemServico.Finalizada
            or StatusOrdemServico.Entregue)
            return;

        if (_servicos.Any(x =>
                x.Status == StatusServico.EmExecucao))
        {
            Status = StatusOrdemServico.EmExecucao;

            if (!DataInicio.HasValue)
                DataInicio = DateTime.UtcNow;
        }
        else if (_servicos.Any()
                 && _servicos.All(x =>
                     x.Status == StatusServico.Finalizada))
        {
            Status = StatusOrdemServico.Finalizada;
            DataFinalizacao = DateTime.UtcNow;
        }
        else
        {
            Status = StatusOrdemServico.AguardandoExecucao;
        }

        RegistrarAlteracao(usuarioId);
    }

    public void Entregar(Guid usuarioId)
    {
        if (Status != StatusOrdemServico.Finalizada)
            throw new DomainException(
                "A ordem de serviço não está finalizada.");

        Status = StatusOrdemServico.Entregue;
        DataEntrega = DateTime.UtcNow;

        RegistrarAlteracao(usuarioId);
    }

    public void CarregarItens(
        IEnumerable<OrdemServicoItemEstoque> itens)
    {
        _itens.Clear();
        _itens.AddRange(itens);
    }

    public void CarregarServicos(
        IEnumerable<OrdemServicoServico> servicos)
    {
        _servicos.Clear();
        _servicos.AddRange(servicos);
    }
}
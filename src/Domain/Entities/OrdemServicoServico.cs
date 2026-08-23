using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class OrdemServicoServico : BaseEntity
{
    public Guid OrdemServicoId { get; private set; }
    public Guid ServicoId { get; private set; }

    public string NomeServico { get; private set; } = string.Empty;
    public string? DescricaoServico { get; private set; }

    public int Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal ValorTotal { get; private set; }

    public StatusServico Status { get; private set; }

    protected OrdemServicoServico()
    {
    }

    // Construtor utilizado na criação da entidade
    public OrdemServicoServico(
        Guid ordemServicoId,
        Guid servicoId,
        string nomeServico,
        string? descricaoServico,
        int quantidade,
        decimal valorUnitario,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        if (valorUnitario <= 0)
            throw new DomainException(
                "O valor unitário deve ser maior que zero.");

        OrdemServicoId = ordemServicoId;
        ServicoId = servicoId;
        NomeServico = nomeServico;
        DescricaoServico = descricaoServico;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = quantidade * valorUnitario;
        Status = StatusServico.AguardandoExecucao;
    }

    // Construtor utilizado pelo mapper ao carregar do banco
    public OrdemServicoServico(
        Guid id,
        Guid ordemServicoId,
        Guid servicoId,
        string nomeServico,
        string? descricaoServico,
        int quantidade,
        decimal valorUnitario,
        decimal valorTotal,
        StatusServico status,
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
        ServicoId = servicoId;
        NomeServico = nomeServico;
        DescricaoServico = descricaoServico;
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        ValorTotal = valorTotal;
        Status = status;
    }

    public void AlterarStatus(
        StatusServico novoStatus,
        Guid usuarioId)
    {
        Status = novoStatus;
        RegistrarAlteracao(usuarioId);
    }
}
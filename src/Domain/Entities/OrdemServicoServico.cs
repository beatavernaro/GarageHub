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

    public DateTime? DataInicio { get; private set; }

    public DateTime? DataFinalizacao { get; private set; }

    // Criação
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

    // Carregamento do banco
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
        DateTime? dataInicio,
        DateTime? dataFinalizacao,
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
        DataInicio = dataInicio;
        DataFinalizacao = dataFinalizacao;
    }

    public void AlterarStatus(
        StatusServico novoStatus,
        Guid usuarioId)
    {
        if (Status == novoStatus)
            return;

        if (Status == StatusServico.Finalizada)
            throw new DomainException(
                "Não é possível alterar um serviço finalizado.");

        if (Status == StatusServico.AguardandoExecucao &&
            novoStatus != StatusServico.EmExecucao)
        {
            throw new DomainException(
                "O serviço deve ser iniciado antes de ser finalizado.");
        }

        if (Status == StatusServico.EmExecucao &&
            novoStatus != StatusServico.Finalizada)
        {
            throw new DomainException(
                "Um serviço em execução só pode ser finalizado.");
        }

        if (novoStatus == StatusServico.EmExecucao)
        {
            DataInicio = DateTime.UtcNow;
        }

        if (novoStatus == StatusServico.Finalizada)
        {
            DataFinalizacao = DateTime.UtcNow;
        }

        Status = novoStatus;

        RegistrarAlteracao(usuarioId);
    }
}
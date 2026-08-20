using Domain.Entities.Base;
using Domain.Exceptions;

namespace Domain.Entities;

public class ServicoItemEstoque : BaseEntity
{
    public ServicoItemEstoque(
        Guid servicoId,
        Guid itemEstoqueId,
        int quantidade,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        Quantidade = quantidade;
    }

    public ServicoItemEstoque(
        Guid id,
        Guid servicoId,
        Guid itemEstoqueId,
        int quantidade,
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
        ServicoId = servicoId;
        ItemEstoqueId = itemEstoqueId;
        Quantidade = quantidade;
    }

    public Guid ServicoId { get; private set; }

    public Guid ItemEstoqueId { get; private set; }

    public int Quantidade { get; private set; }

    public void AlterarQuantidade(
        int quantidade,
        Guid usuarioId)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        Quantidade = quantidade;
        RegistrarAlteracao(usuarioId);
    }
}
using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Servico : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;

    public string? Descricao { get; private set; }

    public decimal Preco { get; private set; }

    public void Normalizar()
    {
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
    }

    public void Atualizar(
        string nome,
        string? descricao,
        Guid usuarioId)
    {
        Nome = nome;
        Descricao = descricao;

        Normalizar();
        RegistrarAlteracao(usuarioId);
    }

    public void AlterarPreco(
        decimal novoPreco,
        Guid usuarioId)
    {
        if (novoPreco <= 0)
            throw new DomainException(
                "O preço deve ser maior que zero.");

        Preco = novoPreco;
        RegistrarAlteracao(usuarioId);
    }

    public Servico(
        string nome,
        string? descricao,
        decimal preco,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
    }

    public Servico(
        Guid id,
        string nome,
        string? descricao,
        decimal preco,
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
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
    }
}
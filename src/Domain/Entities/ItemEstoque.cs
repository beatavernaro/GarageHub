using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class ItemEstoque : BaseEntity
{
    public string CodigoInterno { get; private set; }
    public string Nome { get; private set; }
    public string? Descricao { get; private set; }
    public TipoItemEstoque Tipo { get; private set; }
    public decimal Preco { get; private set; }
    public int Estoque { get; private set; }

    public ItemEstoque(string codigoInterno, string nome, TipoItemEstoque tipo, decimal preco, int estoque, Guid criadoPorId, string? descricao = null) : base(criadoPorId)
    {
        CodigoInterno = codigoInterno;
        Nome = nome;
        Descricao = descricao;
        Tipo = tipo;
        Preco = preco;
        Estoque = estoque;

        Normalizar();
    }

    public ItemEstoque(Guid id, string codigoInterno, string nome, TipoItemEstoque tipo, decimal preco, int estoque, Guid? criadoPorId, DateTime dataCriacao, DateTime? dataAlteracao, Guid? alteradoPorId, bool ativo, string? descricao = null)
        : base(id, dataCriacao, criadoPorId, dataAlteracao, alteradoPorId, ativo)
    {
        CodigoInterno = codigoInterno;
        Nome = nome;
        Descricao = descricao;
        Tipo = tipo;
        Preco = preco;
        Estoque = estoque;
        
        Normalizar();
    }

    public void Normalizar()
    {
        CodigoInterno = CodigoInterno.Trim().ToUpperInvariant();
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
    }

    public void Atualizar(
        string codigoInterno,
        string nome,
        string? descricao,
        TipoItemEstoque tipo)
    {
        CodigoInterno = codigoInterno;
        Nome = nome;
        Descricao = descricao;
        Tipo = tipo;

        Normalizar();
    }
    public void AlterarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new DomainException("O preço deve ser maior que zero.");

        Preco = novoPreco;
    }

    public void AdicionarEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade a ser adicionada deve ser maior que zero.");

        Estoque += quantidade;
    }

    public void RemoverEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade a ser removida deve ser maior que zero.");

        if (Estoque - quantidade < 0)
            throw new DomainException(
                "Não é possível remover mais itens do que o disponível em estoque.");

        Estoque -= quantidade;
    }
}
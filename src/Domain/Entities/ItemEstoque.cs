using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class ItemEstoque(string codigoInterno, string nome, TipoItemEstoque tipo, decimal preco, int estoque, Guid criadoPorId, string? descricao = null) : BaseEntity(criadoPorId)
{
    public string CodigoInterno { get; private set; } = codigoInterno;
    public string Nome { get; private set; } = nome;
    public string? Descricao { get; private set; } = descricao;
    public TipoItemEstoque Tipo { get; private set; } = tipo;
    public decimal Preco { get; private set; } = preco;
    public int Estoque { get; private set; } = estoque;

    public void Normalizar()
    {
        CodigoInterno = CodigoInterno.Trim().ToUpperInvariant();
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
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
            throw new DomainException("A quantidade a ser adicionada deve ser maior que zero.");
        Estoque += quantidade;
    }
    public void RemoverEstoque(int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade a ser removida deve ser maior que zero.");
        if (Estoque - quantidade < 0)
            throw new DomainException("Não é possível remover mais itens do que o disponível em estoque.");
        Estoque -= quantidade;
    }
}
using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Servico(string nome, string? descricao, decimal preco, StatusServico status, Guid criadoPorId) : BaseEntity(criadoPorId)
{
    public string Nome { get; private set; } = nome;
    public string? Descricao { get; private set; } = descricao;
    public decimal Preco { get; private set; } = preco;
    public StatusServico Status { get; private set; } = status;
    private readonly List<ServicoItemEstoque> _itensEstoque = [];
    private readonly Guid criadoPorId = criadoPorId;

    public IReadOnlyCollection<ServicoItemEstoque> ItensEstoque => _itensEstoque;

    public void AlterarPreco(decimal novoPreco)
    {
        if (novoPreco <= 0)
            throw new DomainException("O preço deve ser maior que zero.");
        Preco = novoPreco;
    }

    public void AdicionarPecaInsumo(ItemEstoque item, int quantidade)
    {
        if (quantidade <= 0)
            throw new DomainException("A quantidade deve ser maior que zero.");
        if (!item.Ativo)
            throw new DomainException("O item de estoque deve estar ativo.");

        if (item.Estoque < quantidade)
            throw new DomainException("Não é possível adicionar mais itens do que o disponível em estoque.");

        var itemExistente = _itensEstoque
        .FirstOrDefault(x => x.ItemEstoqueId == item.Id);

        if (itemExistente is not null)
        {
            itemExistente.AlterarQuantidade(itemExistente.Quantidade + quantidade);
            return;
        }

        _itensEstoque.Add(
            new ServicoItemEstoque(Id, item.Id, quantidade, criadoPorId));
    }

    public void Normalizar()
    {
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
    }
}
using Domain.Entities.Base;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class Servico : BaseEntity
{
    private readonly List<ServicoItemEstoque> _itensEstoque = [];

    public Servico(
        string nome,
        string? descricao,
        decimal preco,
        StatusServico status,
        Guid criadoPorId)
        : base(criadoPorId)
    {
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Status = status;
    }

    public Servico(
        Guid id,
        string nome,
        string? descricao,
        decimal preco,
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
        Nome = nome;
        Descricao = descricao;
        Preco = preco;
        Status = status;
    }

    public string Nome { get; private set; } = string.Empty;

    public string? Descricao { get; private set; }

    public decimal Preco { get; private set; }

    public StatusServico Status { get; private set; }

    public IReadOnlyCollection<ServicoItemEstoque> ItensEstoque =>
        _itensEstoque;

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

    public void AlterarStatus(
        StatusServico novoStatus,
        Guid usuarioId)
    {
        Status = novoStatus;
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

    public void AdicionarPecaInsumo(
        ItemEstoque item,
        int quantidade,
        Guid criadoPorId)
    {
        if (quantidade <= 0)
            throw new DomainException(
                "A quantidade deve ser maior que zero.");

        if (!item.Ativo)
            throw new DomainException(
                "O item de estoque deve estar ativo.");

        var itemExistente = _itensEstoque
            .FirstOrDefault(x =>
                x.ItemEstoqueId == item.Id &&
                x.Ativo);

        if (itemExistente is not null)
            throw new DomainException(
                "O item de estoque já está vinculado ao serviço.");

        _itensEstoque.Add(
            new ServicoItemEstoque(
                Id,
                item.Id,
                quantidade,
                criadoPorId));
    }

    public void RemoverItemEstoque(
        Guid itemEstoqueId,
        Guid usuarioId)
    {
        var item = _itensEstoque
            .FirstOrDefault(x =>
                x.ItemEstoqueId == itemEstoqueId &&
                x.Ativo);

        if (item is null)
            throw new DomainException(
                "Item de estoque não encontrado no serviço.");

        item.Desativar(usuarioId);
    }

    public void AlterarQuantidadeItemEstoque(
        Guid itemEstoqueId,
        int quantidade,
        Guid usuarioId)
    {
        var item = _itensEstoque
            .FirstOrDefault(x =>
                x.ItemEstoqueId == itemEstoqueId &&
                x.Ativo);

        if (item is null)
            throw new DomainException(
                "Item de estoque não encontrado no serviço.");

        item.AlterarQuantidade(
            quantidade,
            usuarioId);
    }

    public void CarregarItensEstoque(
        IEnumerable<ServicoItemEstoque> itens)
    {
        _itensEstoque.Clear();
        _itensEstoque.AddRange(itens);
    }
}
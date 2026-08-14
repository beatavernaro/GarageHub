using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class Servico : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public decimal Preco { get; private set; }
    public StatusServico Status { get; private set; }

    private readonly List<ServicoItemEstoque> _itensEstoque = [];
    public IReadOnlyCollection<ServicoItemEstoque> ItensEstoque => _itensEstoque;

    public void Normalizar()
    {
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
    }
}
using Domain.Entities.Base;
using Domain.Enums;

namespace Domain.Entities;

public class ItemEstoque : BaseEntity
{
    public string CodigoInterno { get; private set; } = string.Empty;
    public string Nome { get; private set; } = string.Empty;
    public string? Descricao { get; private set; }
    public TipoItemEstoque Tipo { get; private set; }
    public decimal Preco { get; private set; }
    public int Estoque { get; private set; }

    public void Normalizar()
    {
        CodigoInterno = CodigoInterno.Trim().ToUpperInvariant();
        Nome = Nome.Trim();

        if (!string.IsNullOrWhiteSpace(Descricao))
            Descricao = Descricao.Trim();
    }
}
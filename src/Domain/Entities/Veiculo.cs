using Domain.Entities.Base;

namespace Domain.Entities;

public class Veiculo : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public string Placa { get; private set; } = string.Empty;
    public string? Chassi { get; private set; }
    public string Marca { get; private set; } = string.Empty;
    public string Modelo { get; private set; } = string.Empty;
    public string Cor { get; private set; } = string.Empty;
    public int Ano { get; private set; }
    public int Quilometragem { get; private set; }

    public void Normalizar()
    {
        Placa = Placa
            .Trim()
            .Replace("-", "")
            .ToUpperInvariant();

        if (!string.IsNullOrWhiteSpace(Chassi))
            Chassi = Chassi.Trim().ToUpperInvariant();

        Marca = Marca.Trim();
        Modelo = Modelo.Trim();
        Cor = Cor.Trim();
    }
}
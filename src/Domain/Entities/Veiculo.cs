using Domain.Entities.Base;

namespace Domain.Entities;

public class Veiculo : BaseEntity
{
    public Guid ClienteId { get; private set; }
    public string Placa { get; private set; }
    public string? Chassi { get; private set; }
    public string Marca { get; private set; }
    public string Modelo { get; private set; }
    public string Cor { get; private set; }
    public int Ano { get; private set; }
    public int Quilometragem { get; private set; }

    public Veiculo(Guid clienteId, string placa, string? chassi, string marca, string modelo, string cor, int ano, int quilometragem, Guid criadoPorId) : base(criadoPorId)
    {
        ClienteId = clienteId;
        Placa = placa;
        Chassi = chassi;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        Ano = ano;
        Quilometragem = quilometragem;
    }

    public Veiculo(Guid id, Guid clienteId, string placa, string? chassi, string marca, string modelo, string cor, int ano, int quilometragem, Guid? criadoPorId, DateTime dataCriacao, DateTime? dataAlteracao, Guid? alteradoPorId, bool ativo)
        : base(id, dataCriacao, criadoPorId, dataAlteracao, alteradoPorId, ativo)
    {
        ClienteId = clienteId;
        Placa = placa;
        Chassi = chassi;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        Ano = ano;
        Quilometragem = quilometragem;
    }

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

    public void Atualizar(string placa, string? chassi, string marca, string modelo, string cor, int ano, int quilometragem, Guid usuarioId)
    {
        Placa = placa;
        Chassi = chassi;
        Marca = marca;
        Modelo = modelo;
        Cor = cor;
        Ano = ano;
        Quilometragem = quilometragem;

        Normalizar();
        RegistrarAlteracao(usuarioId);
    }
}
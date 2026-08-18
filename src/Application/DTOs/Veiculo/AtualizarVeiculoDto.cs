namespace Application.DTOs.Veiculo;

public class AtualizarVeiculoDto
{
    public Guid ClienteId { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string? Chassi { get; set; }
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public string Cor { get; set; } = string.Empty;
    public int Ano { get; set; }
    public int Quilometragem { get; set; }
}
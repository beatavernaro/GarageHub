using Domain.Helpers;

namespace Domain.ValueObjects;

public class Endereco(
    string logradouro,
    string numero,
    string? complemento,
    string bairro,
    string cidade,
    string estado,
    string cep)
{
    public string Logradouro { get; private set; } = logradouro;
    public string Numero { get; private set; } = numero;
    public string? Complemento { get; private set; } = complemento;
    public string Bairro { get; private set; } = bairro;
    public string Cidade { get; private set; } = cidade;
    public string Estado { get; private set; } = estado;
    public string Cep { get; private set; } = cep;
    public void Normalizar()
    {
        Cep = NormalizationHelper.NormalizarTexto(Cep);
        Logradouro = NormalizationHelper.NormalizarTexto(Logradouro);
        Numero = Numero.Trim();
        Bairro = NormalizationHelper.NormalizarTexto(Bairro);
        Cidade = NormalizationHelper.NormalizarTexto(Cidade);
        Estado = Estado.Trim().ToUpperInvariant();

        if (!string.IsNullOrWhiteSpace(Complemento))
            Complemento = NormalizationHelper.NormalizarTexto(Complemento);
    }
}
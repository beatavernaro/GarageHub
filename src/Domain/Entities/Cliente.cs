using Domain.Entities.Base;
using Domain.Enums;
using Domain.ValueObjects;
using Domain.Helpers;

namespace Domain.Entities;

public class Cliente(string nome, string cpfCnpj, TipoPessoa tipoPessoa, string telefone, string email) : BaseEntity
{
    public string Nome { get; private set; } = nome;
    public string Documento { get; private set; } = cpfCnpj;
    public TipoPessoa TipoPessoa { get; private set; } = tipoPessoa;
    public string Telefone { get; private set; } = telefone;
    public string Email { get; private set; } = email;
    public Endereco? Endereco { get; private set; }

    public void Normalizar()
    {
        Nome = NormalizationHelper.NormalizarTexto(Nome);
        Documento = NormalizationHelper.NormalizarNumeros(Documento);
        Telefone = NormalizationHelper.NormalizarNumeros(Telefone);
        Email = Email.Trim().ToLowerInvariant();
        Endereco?.Normalizar();
    }
}

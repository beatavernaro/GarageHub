using Domain.Entities.Base;
using Domain.Enums;
using Domain.Helpers;
using Domain.ValueObjects;

namespace Domain.Entities;

public class Cliente : BaseEntity
{
    public string Nome { get; private set; } = string.Empty;
    public string Documento { get; private set; } = string.Empty;
    public TipoPessoa TipoPessoa { get; private set; }
    public string Telefone { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public Endereco? Endereco { get; private set; }

    public void Normalizar()
    {
        Nome = NormalizationHelper.NormalizarTexto(Nome);
        Documento = NormalizationHelper.NormalizarNumeros(Documento);
        Telefone = NormalizationHelper.NormalizarNumeros(Telefone);
        Email = Email.Trim().ToLowerInvariant();

        Endereco?.Normalizar();
    }

    public void Atualizar(
    string nome,
    TipoPessoa tipoPessoa,
    string telefone,
    string email,
    Endereco? endereco,
    Guid usuarioId)
{
    Nome = nome;
    TipoPessoa = tipoPessoa;
    Telefone = telefone;
    Email = email;
    Endereco = endereco;

    Normalizar();

    RegistrarAlteracao(usuarioId);
}

    public Cliente(
        string nome,
        string documento,
        TipoPessoa tipoPessoa,
        string telefone,
        string email,
        Guid criadoPorId,
        Endereco? endereco = null)
        : base(criadoPorId)
    {
        Nome = nome;
        Documento = documento;
        TipoPessoa = tipoPessoa;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;

        Normalizar();
    }

    public Cliente(
        Guid id,
        string nome,
        string documento,
        TipoPessoa tipoPessoa,
        string telefone,
        string email,
        Guid? criadoPorId,
        DateTime dataCriacao,
        DateTime? dataAlteracao,
        Guid? alteradoPorId,
        bool ativo,
        Endereco? endereco = null)
        : base(
            id,
            dataCriacao,
            criadoPorId,
            dataAlteracao,
            alteradoPorId,
            ativo)
    {
        Nome = nome;
        Documento = documento;
        TipoPessoa = tipoPessoa;
        Telefone = telefone;
        Email = email;
        Endereco = endereco;

        Normalizar();
    }

}
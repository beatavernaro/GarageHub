using Domain.Entities;
using Domain.Enums;
using Elekto.BrazilianDocuments;
using FluentValidation;

namespace Domain.Validators;

public class ClienteValidator : AbstractValidator<Cliente>
{
    public ClienteValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(3)
            .WithMessage("O nome deve possuir pelo menos 3 caracteres.");

        RuleFor(x => x.Documento)
            .NotEmpty()
            .Must((cliente, documento) => ValidarDocumento(documento, cliente.TipoPessoa))
            .WithMessage("CPF/CNPJ inválido.");

        RuleFor(x => x.TipoPessoa)
            .IsInEnum()
            .WithMessage("Tipo de pessoa inválido.");

        RuleFor(x => x.Telefone)
            .NotEmpty()
            .Matches(@"^\d{10,11}$")
            .WithMessage("O telefone deve possuir entre 10 e 11 dígitos.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("E-mail inválido.");


        ValidarEndereco();

    }
    private static bool ValidarDocumento(string documento, TipoPessoa tipoPessoa)
    {
        var tipoDocumento = tipoPessoa == TipoPessoa.Fisica
            ? DocumentType.Cpf
            : DocumentType.Cnpj;

        return BrazilianDocument.IsValid(documento, tipoDocumento).IsValid;
    }

    private void ValidarEndereco()
    {
        When(x => x.Endereco != null, () =>
        {
            RuleFor(x => x.Endereco!.Cep)
                .NotEmpty()
                .Matches(@"^\d{8}$")
                .WithMessage("CEP inválido.");

            RuleFor(x => x.Endereco!.Logradouro)
                .NotEmpty()
                .WithMessage("Logradouro é obrigatório.");

            RuleFor(x => x.Endereco!.Numero)
                .NotEmpty()
                .WithMessage("Número é obrigatório.");

            RuleFor(x => x.Endereco!.Bairro)
                .NotEmpty()
                .WithMessage("Bairro é obrigatório.");

            RuleFor(x => x.Endereco!.Cidade)
                .NotEmpty()
                .WithMessage("Cidade é obrigatória.");

            RuleFor(x => x.Endereco!.Estado)
                .NotEmpty()
                .Length(2)
                .WithMessage("Estado deve possuir 2 caracteres.");
        });
    }
}
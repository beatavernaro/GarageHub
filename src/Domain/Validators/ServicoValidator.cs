using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class ServicoValidator : AbstractValidator<Servico>
{
    public ServicoValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("O nome deve possuir pelo menos 2 caracteres.");

        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.");

    }
}
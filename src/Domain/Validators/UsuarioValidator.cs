using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class UsuarioValidator : AbstractValidator<Usuario>
{
    public UsuarioValidator()
    {
        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("O nome deve possuir pelo menos 2 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("O email deve ser válido.");

        RuleFor(x => x.SenhaHash)
            .NotEmpty()
            .MinimumLength(6)
            .WithMessage("A senha deve possuir pelo menos 6 caracteres.");
    }
}
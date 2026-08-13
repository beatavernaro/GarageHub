using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class VeiculoValidator : AbstractValidator<Veiculo>
{
    public VeiculoValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty()
            .WithMessage("O cliente é obrigatório.");

        RuleFor(x => x.Placa)
            .NotEmpty()
            .Length(7)
            .Matches(@"^[A-Z]{3}[0-9][A-Z0-9][0-9]{2}$")
            .WithMessage("A placa informada é inválida.");

        RuleFor(x => x.Chassi)
            .Length(17)
            .When(x => !string.IsNullOrWhiteSpace(x.Chassi))
            .WithMessage("O chassi deve possuir 17 caracteres.");

        RuleFor(x => x.Marca)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("A marca deve possuir pelo menos 2 caracteres.");

        RuleFor(x => x.Modelo)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("O modelo deve possuir pelo menos 2 caracteres.");

        RuleFor(x => x.Cor)
            .NotEmpty()
            .WithMessage("A cor é obrigatória.");

        RuleFor(x => x.Ano)
            .InclusiveBetween(1886, DateTime.Now.Year + 1)
            .WithMessage("Ano do veículo inválido.");

        RuleFor(x => x.Quilometragem)
            .GreaterThanOrEqualTo(0)
            .WithMessage("A quilometragem não pode ser negativa.");
    }
}
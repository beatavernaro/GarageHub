using FluentValidation;
using Domain.Entities;

namespace Domain.Validators;

public class OrdemServicoItemValidator : AbstractValidator<OrdemServicoItem>
{
    public OrdemServicoItemValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty()
            .WithMessage("A ordem de serviço é obrigatória.");

        RuleFor(x => x.ServicoId)
            .NotEmpty()
            .WithMessage("O serviço é obrigatório.");

        RuleFor(x => x.NomeServico)
            .NotEmpty()
            .WithMessage("O nome do serviço é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(x => x.ValorUnitario)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O valor unitário não pode ser negativo.");

        RuleFor(x => x.ValorTotal)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O valor total não pode ser negativo.");
    }
}
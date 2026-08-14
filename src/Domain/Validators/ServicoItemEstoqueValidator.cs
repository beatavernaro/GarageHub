using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class ServicoItemEstoqueValidator : AbstractValidator<ServicoItemEstoque>
{
    public ServicoItemEstoqueValidator()
    {
        RuleFor(x => x.ServicoId)
            .NotEmpty()
            .WithMessage("O serviço é obrigatório.");

        RuleFor(x => x.ItemEstoqueId)
            .NotEmpty()
            .WithMessage("O item de estoque é obrigatório.");

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");
    }
}
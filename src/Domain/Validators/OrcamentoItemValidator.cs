using Domain.Entities;
using FluentValidation;

namespace GarageHub.Domain.Validators;

public class OrcamentoItemValidator : AbstractValidator<OrcamentoItem>
{
    public OrcamentoItemValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty()
            .WithMessage("O orçamento é obrigatório.");

        RuleFor(x => x)
            .Must(x => (x.ServicoId.HasValue && !x.ItemEstoqueId.HasValue) ||
                       (!x.ServicoId.HasValue && x.ItemEstoqueId.HasValue))
            .WithMessage("O item deve possuir um serviço ou um item de estoque.");

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
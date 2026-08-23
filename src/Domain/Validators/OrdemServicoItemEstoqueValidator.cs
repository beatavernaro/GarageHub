using FluentValidation;
using Domain.Entities;

namespace Domain.Validators;

public class OrdemServicoItemEstoqueValidator : AbstractValidator<OrdemServicoItemEstoque>
{
    public OrdemServicoItemEstoqueValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty()
            .WithMessage("A ordem de serviço é obrigatória.");

        RuleFor(x => x.ItemEstoqueId)
            .NotEmpty()
            .WithMessage("O item de estoque é obrigatório.");

        RuleFor(x => x.NomeItem)
            .NotEmpty()
            .WithMessage("O nome do item é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do item deve ter no máximo 200 caracteres.");

        RuleFor(x => x.DescricaoItem)
            .MaximumLength(500)
            .WithMessage("A descrição do item deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DescricaoItem));

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0)
            .WithMessage("O valor unitário deve ser maior que zero.");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0)
            .WithMessage("O valor total deve ser maior que zero.");

        RuleFor(x => x.ValorTotal)
            .Must((item, valorTotal) =>
                valorTotal == item.Quantidade * item.ValorUnitario)
            .WithMessage(
                "O valor total deve corresponder à quantidade multiplicada pelo valor unitário.");
    }
}
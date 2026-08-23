using FluentValidation;
using Domain.Entities;

namespace Domain.Validators;

public class OrcamentoValidator : AbstractValidator<Orcamento>
{
    public OrcamentoValidator()
    {
        RuleFor(x => x.ClienteId)
            .NotEmpty()
            .WithMessage("O cliente é obrigatório.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty()
            .WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status do orçamento inválido.");

        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("O orçamento deve possuir pelo menos um item.");

        RuleForEach(x => x.Itens)
            .SetValidator(new OrcamentoItemValidator());

        RuleFor(x => x.Desconto)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O desconto não pode ser negativo.");

        RuleFor(x => x)
            .Must(x => x.Desconto <= CalcularSubtotal(x))
            .WithMessage("O desconto não pode ser maior que o valor dos itens.");
    }

    private static decimal CalcularSubtotal(Orcamento orcamento)
    {
        return orcamento.Itens.Sum(x => x.ValorTotal);
    }
}
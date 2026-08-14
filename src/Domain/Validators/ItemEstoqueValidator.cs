using Domain.Entities;
using FluentValidation;

namespace Domain.Validators;

public class ItemEstoqueValidator : AbstractValidator<ItemEstoque>
{
    public ItemEstoqueValidator()
    {
        RuleFor(x => x.CodigoInterno)
            .NotEmpty()
            .WithMessage("O código interno é obrigatório.");

        RuleFor(x => x.Nome)
            .NotEmpty()
            .MinimumLength(2)
            .WithMessage("O nome deve possuir pelo menos 2 caracteres.");

        RuleFor(x => x.Tipo)
            .IsInEnum()
            .WithMessage("O tipo do item é inválido.");

        RuleFor(x => x.Preco)
            .GreaterThan(0)
            .WithMessage("O preço deve ser maior que zero.");

        RuleFor(x => x.Estoque)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O estoque não pode ser negativo.");
    }
}
using FluentValidation;

namespace Domain.Validators;

public class OrdemServicoServicoValidator : AbstractValidator<OrdemServicoServico>
{
    public OrdemServicoServicoValidator()
    {
        RuleFor(x => x.OrdemServicoId)
            .NotEmpty()
            .WithMessage("A ordem de serviço é obrigatória.");

        RuleFor(x => x.ServicoId)
            .NotEmpty()
            .WithMessage("O serviço é obrigatório.");

        RuleFor(x => x.NomeServico)
            .NotEmpty()
            .WithMessage("O nome do serviço é obrigatório.")
            .MaximumLength(200)
            .WithMessage("O nome do serviço deve ter no máximo 200 caracteres.");

        RuleFor(x => x.DescricaoServico)
            .MaximumLength(500)
            .WithMessage("A descrição do serviço deve ter no máximo 500 caracteres.")
            .When(x => !string.IsNullOrWhiteSpace(x.DescricaoServico));

        RuleFor(x => x.Quantidade)
            .GreaterThan(0)
            .WithMessage("A quantidade deve ser maior que zero.");

        RuleFor(x => x.ValorUnitario)
            .GreaterThan(0)
            .WithMessage("O valor unitário deve ser maior que zero.");

        RuleFor(x => x.ValorTotal)
            .GreaterThan(0)
            .WithMessage("O valor total deve ser maior que zero.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status do serviço inválido.");

        RuleFor(x => x.ValorTotal)
            .Must((item, valorTotal) =>
                valorTotal == item.Quantidade * item.ValorUnitario)
            .WithMessage("O valor total deve corresponder à quantidade multiplicada pelo valor unitário.");
    }
}
using FluentValidation;
using Domain.Entities;

namespace Domain.Validators;

public class OrdemServicoValidator : AbstractValidator<OrdemServico>
{
    public OrdemServicoValidator()
    {
        RuleFor(x => x.OrcamentoId)
            .NotEmpty()
            .WithMessage("O orçamento é obrigatório.");

        RuleFor(x => x.ClienteId)
            .NotEmpty()
            .WithMessage("O cliente é obrigatório.");

        RuleFor(x => x.VeiculoId)
            .NotEmpty()
            .WithMessage("O veículo é obrigatório.");

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Status da ordem de serviço inválido.");

        RuleFor(x => x.Itens)
            .NotEmpty()
            .WithMessage("A ordem de serviço deve possuir pelo menos um serviço.");

        RuleForEach(x => x.Itens)
            .SetValidator(new OrdemServicoItemEstoqueValidator());

        RuleFor(x => x.Desconto)
            .GreaterThanOrEqualTo(0)
            .WithMessage("O desconto não pode ser negativo.");

        RuleFor(x => x)
            .Must(x => x.Desconto <= x.Itens.Sum(i => i.ValorTotal))
            .WithMessage("O desconto não pode ser maior que o valor dos serviços.");

        RuleFor(x => x.DataFinalizacao)
            .GreaterThanOrEqualTo(x => x.DataInicio)
            .When(x => x.DataInicio.HasValue && x.DataFinalizacao.HasValue)
            .WithMessage("A finalização não pode ser anterior ao início.");

        RuleFor(x => x.DataEntrega)
            .GreaterThanOrEqualTo(x => x.DataFinalizacao)
            .When(x => x.DataFinalizacao.HasValue && x.DataEntrega.HasValue)
            .WithMessage("A entrega não pode ser anterior à finalização.");
    }
}
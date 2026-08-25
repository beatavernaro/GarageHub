using Domain.Entities;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class OrcamentoItemValidatorTests
{
    private readonly OrcamentoItemValidator _validator = new();

    [Fact]
    public void Deve_Validar_Item_Valido()
    {
        var item = CriarItem();

        _validator.Validate(item)
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_OrcamentoId_Vazio()
    {
        var item = CriarItem(
            orcamentoId: Guid.Empty);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O orçamento é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Quantidade_Zero()
    {
        var item = CriarItem(
            quantidade: 0);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A quantidade deve ser maior que zero.");
    }

    [Fact]
    public void Nao_Deve_Validar_Valor_Unitario_Negativo()
    {
        var item = CriarItem(
            valorUnitario: -1);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O valor unitário não pode ser negativo.");
    }

    [Fact]
    public void Nao_Deve_Validar_Valor_Total_Negativo()
    {
        var item = CriarItem(
            valorTotal: -1);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O valor total não pode ser negativo.");
    }

    private static OrcamentoItem CriarItem(
        Guid? orcamentoId = null,
        int quantidade = 1,
        decimal valorUnitario = 100m,
        decimal valorTotal = 100m)
    {
        return new OrcamentoItem(
            Guid.NewGuid(),
            orcamentoId ?? Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Troca de Óleo",
            null,
            quantidade,
            valorUnitario,
            valorTotal,
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            null,
            true);
    }
}
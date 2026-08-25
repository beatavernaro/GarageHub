using Domain.Entities;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class OrdemServicoItemEstoqueValidatorTests
{
    private readonly OrdemServicoItemEstoqueValidator _validator = new();

    [Fact]
    public void Deve_Validar_Item_Valido()
    {
        var item = CriarItem();

        _validator.Validate(item)
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Ordem_Vazia()
    {
        var item = CriarItem(
            ordemServicoId: Guid.Empty);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A ordem de serviço é obrigatória.");
    }

    [Fact]
    public void Nao_Deve_Validar_ItemEstoqueId_Vazio()
    {
        var item = CriarItem(
            itemEstoqueId: Guid.Empty);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O item de estoque é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Nome_Vazio()
    {
        var item = CriarItem(nome: "");

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O nome do item é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Nome_Maior_Que_200()
    {
        var item = CriarItem(
            nome: new string('A', 201));

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O nome do item deve ter no máximo 200 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Descricao_Maior_Que_500()
    {
        var item = CriarItem(
            descricao: new string('A', 501));

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A descrição do item deve ter no máximo 500 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Quantidade_Invalida()
    {
        var item = CriarItem(
            quantidade: 0,
            valorTotal: 0);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.PropertyName ==
                nameof(OrdemServicoItemEstoque.Quantidade));
    }

    [Fact]
    public void Nao_Deve_Validar_Valor_Unitario_Invalido()
    {
        var item = CriarItem(
            valorUnitario: 0,
            valorTotal: 0);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.PropertyName ==
                nameof(OrdemServicoItemEstoque.ValorUnitario));
    }

    [Fact]
    public void Nao_Deve_Validar_Total_Incorreto()
    {
        var item = CriarItem(
            quantidade: 2,
            valorUnitario: 100,
            valorTotal: 150);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage.Contains(
                    "quantidade multiplicada"));
    }

    private static OrdemServicoItemEstoque CriarItem(
        Guid? ordemServicoId = null,
        Guid? itemEstoqueId = null,
        string nome = "Óleo",
        string? descricao = null,
        int quantidade = 1,
        decimal valorUnitario = 100,
        decimal valorTotal = 100)
    {
        return new OrdemServicoItemEstoque(
            Guid.NewGuid(),
            ordemServicoId ?? Guid.NewGuid(),
            itemEstoqueId ?? Guid.NewGuid(),
            nome,
            descricao,
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
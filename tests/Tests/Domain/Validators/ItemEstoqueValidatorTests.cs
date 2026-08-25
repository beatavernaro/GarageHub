using Domain.Entities;
using Domain.Enums;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class ItemEstoqueValidatorTests
{
    private readonly ItemEstoqueValidator _validator = new();

    [Fact]
    public void Deve_Validar_Item_Valido()
    {
        var item = CriarItem();

        var resultado = _validator.Validate(item);

        resultado.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("ABC123")]
    [InlineData("AB12345")]
    [InlineData("1234567")]
    public void Nao_Deve_Validar_Codigo_Invalido(string codigo)
    {
        var item = CriarItem(codigo: codigo);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.PropertyName == nameof(ItemEstoque.CodigoInterno));
    }

    [Fact]
    public void Nao_Deve_Validar_Nome_Curto()
    {
        var item = CriarItem(nome: "A");

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O nome deve possuir pelo menos 2 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Tipo_Invalido()
    {
        var item = CriarItem(
            tipo: (TipoItemEstoque)999);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O tipo do item é inválido.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Validar_Preco_Invalido(decimal preco)
    {
        var item = CriarItem(preco: preco);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O preço deve ser maior que zero.");
    }

    [Fact]
    public void Nao_Deve_Validar_Estoque_Negativo()
    {
        var item = CriarItem(estoque: -1);

        var resultado = _validator.Validate(item);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O estoque não pode ser negativo.");
    }

    private static ItemEstoque CriarItem(
        string codigo = "PEC0001",
        string nome = "Pastilha",
        TipoItemEstoque tipo = TipoItemEstoque.Peca,
        decimal preco = 100m,
        int estoque = 10)
    {
        return new ItemEstoque(
            codigo,
            nome,
            tipo,
            preco,
            estoque,
            Guid.NewGuid());
    }
}
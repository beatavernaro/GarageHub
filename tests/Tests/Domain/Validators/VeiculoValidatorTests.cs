using Domain.Entities;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class VeiculoValidatorTests
{
    private readonly VeiculoValidator _validator = new();

    [Fact]
    public void Deve_Validar_Veiculo_Valido()
    {
        var veiculo = CriarVeiculo();

        var resultado = _validator.Validate(veiculo);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_ClienteId_Vazio()
    {
        var veiculo = CriarVeiculo(
            clienteId: Guid.Empty);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O cliente é obrigatório.");
    }

    [Theory]
    [InlineData("ABC123")]
    [InlineData("ABCD123")]
    [InlineData("1234ABC")]
    [InlineData("ABC@123")]
    public void Nao_Deve_Validar_Placa_Invalida(
        string placa)
    {
        var veiculo = CriarVeiculo(
            placa: placa);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.PropertyName == nameof(Veiculo.Placa));
    }

    [Theory]
    [InlineData("ABC1234")]
    [InlineData("ABC1D23")]
    public void Deve_Validar_Placa_Valida(
        string placa)
    {
        var veiculo = CriarVeiculo(
            placa: placa);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.PropertyName == nameof(Veiculo.Placa));
    }

    [Fact]
    public void Deve_Permitir_Chassi_Nulo()
    {
        var veiculo = CriarVeiculo(
            chassi: null);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.PropertyName == nameof(Veiculo.Chassi));
    }

    [Fact]
    public void Nao_Deve_Validar_Chassi_Com_Tamanho_Invalido()
    {
        var veiculo = CriarVeiculo(
            chassi: "123456");

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O chassi deve possuir 17 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Marca_Com_Menos_De_Dois_Caracteres()
    {
        var veiculo = CriarVeiculo(
            marca: "A");

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A marca deve possuir pelo menos 2 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Modelo_Com_Menos_De_Dois_Caracteres()
    {
        var veiculo = CriarVeiculo(
            modelo: "A");

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O modelo deve possuir pelo menos 2 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Cor_Vazia()
    {
        var veiculo = CriarVeiculo(
            cor: "");

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A cor é obrigatória.");
    }

    [Fact]
    public void Nao_Deve_Validar_Ano_Anterior_A_1886()
    {
        var veiculo = CriarVeiculo(
            ano: 1885);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Ano do veículo inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Ano_Maior_Que_Proximo_Ano()
    {
        var veiculo = CriarVeiculo(
            ano: DateTime.Now.Year + 2);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Ano do veículo inválido.");
    }

    [Fact]
    public void Deve_Validar_Proximo_Ano()
    {
        var veiculo = CriarVeiculo(
            ano: DateTime.Now.Year + 1);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.PropertyName == nameof(Veiculo.Ano));
    }

    [Fact]
    public void Nao_Deve_Validar_Quilometragem_Negativa()
    {
        var veiculo = CriarVeiculo(
            quilometragem: -1);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A quilometragem não pode ser negativa.");
    }

    [Fact]
    public void Deve_Permitir_Quilometragem_Zero()
    {
        var veiculo = CriarVeiculo(
            quilometragem: 0);

        var resultado = _validator.Validate(veiculo);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.PropertyName ==
                nameof(Veiculo.Quilometragem));
    }

    private static Veiculo CriarVeiculo(
        Guid? clienteId = null,
        string placa = "ABC1D23",
        string? chassi = "9BWZZZ377VT004251",
        string marca = "Volkswagen",
        string modelo = "Gol",
        string cor = "Prata",
        int ano = 2020,
        int quilometragem = 45000)
    {
        return new Veiculo(
            clienteId ?? Guid.NewGuid(),
            placa,
            chassi,
            marca,
            modelo,
            cor,
            ano,
            quilometragem,
            Guid.NewGuid());
    }
}
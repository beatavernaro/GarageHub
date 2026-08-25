using Domain.Entities;
using Domain.Enums;
using Domain.Validators;
using Domain.ValueObjects;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class ClienteValidatorTests
{
    private readonly ClienteValidator _validator = new();

    [Fact]
    public void Deve_Validar_Cliente_Valido()
    {
        var cliente = CriarClienteValido();

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Nome_Com_Menos_De_3_Caracteres()
    {
        var cliente = CriarCliente(
            nome: "Jo");

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O nome deve possuir pelo menos 3 caracteres.");
    }

    [Fact]
    public void Nao_Deve_Validar_Cpf_Invalido()
    {
        var cliente = CriarCliente(
            documento: "12345678901",
            tipoPessoa: TipoPessoa.Fisica);

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "CPF/CNPJ inválido.");
    }

    [Fact]
    public void Deve_Validar_Cpf_Valido()
    {
        var cliente = CriarCliente(
            documento: "52998224725",
            tipoPessoa: TipoPessoa.Fisica);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.ErrorMessage == "CPF/CNPJ inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Cnpj_Invalido()
    {
        var cliente = CriarCliente(
            documento: "12345678000199",
            tipoPessoa: TipoPessoa.Juridica);

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeFalse();
        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "CPF/CNPJ inválido.");
    }

    [Fact]
    public void Deve_Validar_Cnpj_Valido()
    {
        var cliente = CriarCliente(
            documento: "11222333000181",
            tipoPessoa: TipoPessoa.Juridica);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.ErrorMessage == "CPF/CNPJ inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Tipo_Pessoa_Invalido()
    {
        var cliente = CriarCliente(
            tipoPessoa: (TipoPessoa)999);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Tipo de pessoa inválido.");
    }

    [Theory]
    [InlineData("123")]
    [InlineData("123456789")]
    [InlineData("123456789012")]
    public void Nao_Deve_Validar_Telefone_Invalido(
        string telefone)
    {
        var cliente = CriarCliente(
            telefone: telefone);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O telefone deve possuir entre 10 e 11 dígitos.");
    }

    [Theory]
    [InlineData("1599999000")]
    [InlineData("15999990001")]
    public void Deve_Validar_Telefone_Valido(
        string telefone)
    {
        var cliente = CriarCliente(
            telefone: telefone);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .NotContain(x =>
                x.PropertyName == nameof(Cliente.Telefone));
    }

    [Fact]
    public void Nao_Deve_Validar_Email_Invalido()
    {
        var cliente = CriarCliente(
            email: "email-invalido");

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "E-mail inválido.");
    }

    [Fact]
    public void Deve_Validar_Cliente_Sem_Endereco()
    {
        var cliente = CriarClienteValido(
            endereco: null);

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Deve_Validar_Endereco_Valido()
    {
        var cliente = CriarClienteValido(
            endereco: CriarEndereco());

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Cep_Invalido()
    {
        var endereco = new Endereco(
            "Rua das Flores",
            "100",
            null,
            "Centro",
            "Sorocaba",
            "SP",
            "123");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage == "CEP inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Logradouro_Vazio()
    {
        var endereco = new Endereco(
            "",
            "100",
            null,
            "Centro",
            "Sorocaba",
            "SP",
            "18000000");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Logradouro é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Numero_Vazio()
    {
        var endereco = new Endereco(
            "Rua das Flores",
            "",
            null,
            "Centro",
            "Sorocaba",
            "SP",
            "18000000");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Número é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Bairro_Vazio()
    {
        var endereco = new Endereco(
            "Rua das Flores",
            "100",
            null,
            "",
            "Sorocaba",
            "SP",
            "18000000");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Bairro é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Cidade_Vazia()
    {
        var endereco = new Endereco(
            "Rua das Flores",
            "100",
            null,
            "Centro",
            "",
            "SP",
            "18000000");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Cidade é obrigatória.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("S")]
    [InlineData("SPO")]
    public void Nao_Deve_Validar_Estado_Invalido(
        string estado)
    {
        var endereco = new Endereco(
            "Rua das Flores",
            "100",
            null,
            "Centro",
            "Sorocaba",
            estado,
            "18000000");

        var cliente = CriarClienteValido(
            endereco: endereco);

        var resultado = _validator.Validate(cliente);

        resultado.IsValid.Should().BeFalse();
    }

    private static Cliente CriarClienteValido(
        Endereco? endereco = null)
    {
        return CriarCliente(
            documento: "52998224725",
            endereco: endereco);
    }

    private static Cliente CriarCliente(
        string nome = "João da Silva",
        string documento = "52998224725",
        TipoPessoa tipoPessoa = TipoPessoa.Fisica,
        string telefone = "15999990001",
        string email = "joao@email.com",
        Endereco? endereco = null)
    {
        return new Cliente(
            nome,
            documento,
            tipoPessoa,
            telefone,
            email,
            Guid.NewGuid(),
            endereco);
    }

    private static Endereco CriarEndereco()
    {
        return new Endereco(
            "Rua das Flores",
            "100",
            null,
            "Centro",
            "Sorocaba",
            "SP",
            "18000000");
    }
}
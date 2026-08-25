using Domain.Entities;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class ServicoValidatorTests
{
    private readonly ServicoValidator _validator = new();

    [Fact]
    public void Deve_Validar_Servico_Valido()
    {
        var servico = CriarServico();

        _validator.Validate(servico)
            .IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nao_Deve_Validar_Nome_Invalido(string nome)
    {
        var servico = CriarServico(nome: nome);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.PropertyName == nameof(Servico.Nome));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Validar_Preco_Invalido(decimal preco)
    {
        var servico = CriarServico(preco: preco);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O preço deve ser maior que zero.");
    }

    [Theory]
    [InlineData("")]
    [InlineData("SER001")]
    [InlineData("1234567")]
    public void Nao_Deve_Validar_Codigo_Invalido(string codigo)
    {
        var servico = CriarServico(codigo: codigo);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.PropertyName ==
                nameof(Servico.CodigoInterno));
    }

    private static Servico CriarServico(
        string codigo = "SER0001",
        string nome = "Troca de Óleo",
        decimal preco = 100m)
    {
        return new Servico(
            codigo,
            nome,
            null,
            preco,
            Guid.NewGuid());
    }
}
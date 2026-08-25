using Domain.Entities;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class UsuarioValidatorTests
{
    private readonly UsuarioValidator _validator = new();

    [Fact]
    public void Deve_Validar_Usuario_Valido()
    {
        var usuario = CriarUsuario();

        _validator.Validate(usuario)
            .IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("A")]
    public void Nao_Deve_Validar_Nome_Invalido(string nome)
    {
        var usuario = CriarUsuario(nome: nome);

        _validator.Validate(usuario).Errors
            .Should()
            .Contain(x =>
                x.PropertyName == nameof(Usuario.Nome));
    }

    [Theory]
    [InlineData("")]
    [InlineData("email-invalido")]
    public void Nao_Deve_Validar_Email_Invalido(string email)
    {
        var usuario = CriarUsuario(email: email);

        _validator.Validate(usuario).Errors
            .Should()
            .Contain(x =>
                x.PropertyName == nameof(Usuario.Email));
    }

    [Theory]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("12345")]
    public void Nao_Deve_Validar_SenhaHash_Invalida(
        string senha)
    {
        var usuario = CriarUsuario(
            senhaHash: senha);

        _validator.Validate(usuario).Errors
            .Should()
            .Contain(x =>
                x.PropertyName ==
                nameof(Usuario.SenhaHash));
    }

    private static Usuario CriarUsuario(
        string nome = "Administrador",
        string email = "admin@garagehub.com",
        string senhaHash = "123456")
    {
        return new Usuario(
            nome,
            email,
            senhaHash,
            Guid.NewGuid());
    }
}
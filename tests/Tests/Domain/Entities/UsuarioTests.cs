using Domain.Entities;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class UsuarioTests
{
    [Fact]
    public void Deve_Criar_Usuario_Com_Dados_Validos()
    {
        var criadoPorId = Guid.NewGuid();

        var usuario = new Usuario(
            "Administrador",
            "admin@garagehub.com",
            "hash",
            criadoPorId);

        usuario.Id.Should().NotBeEmpty();
        usuario.Nome.Should().Be("Administrador");
        usuario.Email.Should().Be("admin@garagehub.com");
        usuario.SenhaHash.Should().Be("hash");
        usuario.CriadoPorId.Should().Be(criadoPorId);
        usuario.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Deve_Normalizar_Nome_E_Email()
    {
        var usuario = new Usuario(
            " Administrador ",
            " ADMIN@GARAGEHUB.COM ",
            "hash",
            Guid.NewGuid());

        usuario.Nome.Should().Be("Administrador");
        usuario.Email.Should().Be("admin@garagehub.com");
    }

    [Fact]
    public void Deve_Reconstruir_Usuario_Com_Auditoria()
    {
        var id = Guid.NewGuid();
        var criadoPorId = Guid.NewGuid();
        var alteradoPorId = Guid.NewGuid();
        var dataCriacao =
            DateTime.UtcNow.AddDays(-2);
        var dataAlteracao =
            DateTime.UtcNow.AddDays(-1);

        var usuario = new Usuario(
            id,
            "Administrador",
            "admin@garagehub.com",
            "hash",
            criadoPorId,
            dataCriacao,
            dataAlteracao,
            alteradoPorId,
            false);

        usuario.Id.Should().Be(id);
        usuario.DataCriacao.Should().Be(dataCriacao);
        usuario.CriadoPorId.Should().Be(criadoPorId);
        usuario.DataAlteracao.Should().Be(dataAlteracao);
        usuario.AlteradoPorId.Should().Be(alteradoPorId);
        usuario.Ativo.Should().BeFalse();
    }
}
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class ClienteTests
{
    [Fact]
    public void Deve_Criar_Cliente_Com_Dados_Validos()
    {
        var criadoPorId = Guid.NewGuid();

        var cliente = new Cliente(
            "João da Silva",
            "12345678901",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            criadoPorId);

        cliente.Id.Should().NotBeEmpty();
        cliente.Nome.Should().Be("João da Silva");
        cliente.Documento.Should().Be("12345678901");
        cliente.TipoPessoa.Should().Be(TipoPessoa.Fisica);
        cliente.Telefone.Should().Be("15999990001");
        cliente.Email.Should().Be("joao@email.com");
        cliente.CriadoPorId.Should().Be(criadoPorId);
        cliente.DataCriacao.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(2));
        cliente.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Deve_Normalizar_Dados_Ao_Criar_Cliente()
    {
        var cliente = new Cliente(
            "  João da Silva  ",
            "123.456.789-01",
            TipoPessoa.Fisica,
            "(15) 99999-0001",
            "  JOAO@EMAIL.COM  ",
            Guid.NewGuid());

        cliente.Nome.Should().Be("João da Silva");
        cliente.Documento.Should().Be("12345678901");
        cliente.Telefone.Should().Be("15999990001");
        cliente.Email.Should().Be("joao@email.com");
    }

    [Fact]
    public void Deve_Criar_Cliente_Com_Endereco()
    {
        var endereco = CriarEndereco();

        var cliente = new Cliente(
            "João da Silva",
            "12345678901",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid(),
            endereco);

        cliente.Endereco.Should().NotBeNull();
        cliente.Endereco.Should().BeSameAs(endereco);
    }

    [Fact]
    public void Deve_Atualizar_Dados_Do_Cliente()
    {
        var cliente = CriarCliente();
        var usuarioId = Guid.NewGuid();
        var endereco = CriarEndereco();

        cliente.Atualizar(
            "Maria da Silva",
            TipoPessoa.Juridica,
            "1533334444",
            "maria@email.com",
            endereco,
            usuarioId);

        cliente.Nome.Should().Be("Maria da Silva");
        cliente.TipoPessoa.Should().Be(TipoPessoa.Juridica);
        cliente.Telefone.Should().Be("1533334444");
        cliente.Email.Should().Be("maria@email.com");
        cliente.Endereco.Should().BeSameAs(endereco);
    }

    [Fact]
    public void Deve_Normalizar_Dados_Ao_Atualizar_Cliente()
    {
        var cliente = CriarCliente();

        cliente.Atualizar(
            "  Maria da Silva  ",
            TipoPessoa.Fisica,
            "(15) 98888-7777",
            "  MARIA@EMAIL.COM  ",
            null,
            Guid.NewGuid());

        cliente.Nome.Should().Be("Maria da Silva");
        cliente.Telefone.Should().Be("15988887777");
        cliente.Email.Should().Be("maria@email.com");
        cliente.Endereco.Should().BeNull();
    }

    [Fact]
    public void Deve_Registrar_Usuario_Que_Alterou_Cliente()
    {
        var cliente = CriarCliente();
        var usuarioId = Guid.NewGuid();

        cliente.Atualizar(
            "Maria da Silva",
            TipoPessoa.Fisica,
            "15999990002",
            "maria@email.com",
            null,
            usuarioId);

        cliente.AlteradoPorId.Should().Be(usuarioId);
        cliente.DataAlteracao.Should().NotBeNull();
        cliente.DataAlteracao.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Atualizar_Nao_Deve_Alterar_Documento()
    {
        var cliente = CriarCliente();

        cliente.Atualizar(
            "Maria da Silva",
            TipoPessoa.Fisica,
            "15999990002",
            "maria@email.com",
            null,
            Guid.NewGuid());

        cliente.Documento.Should().Be("12345678901");
    }

    [Fact]
    public void Deve_Reconstruir_Cliente_Com_Dados_De_Auditoria()
    {
        var id = Guid.NewGuid();
        var criadoPorId = Guid.NewGuid();
        var alteradoPorId = Guid.NewGuid();
        var dataCriacao = DateTime.UtcNow.AddDays(-5);
        var dataAlteracao = DateTime.UtcNow.AddDays(-1);

        var cliente = new Cliente(
            id,
            "João da Silva",
            "12345678901",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            criadoPorId,
            dataCriacao,
            dataAlteracao,
            alteradoPorId,
            false);

        cliente.Id.Should().Be(id);
        cliente.CriadoPorId.Should().Be(criadoPorId);
        cliente.DataCriacao.Should().Be(dataCriacao);
        cliente.AlteradoPorId.Should().Be(alteradoPorId);
        cliente.DataAlteracao.Should().Be(dataAlteracao);
        cliente.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Deve_Desativar_Cliente()
    {
        var cliente = CriarCliente();
        var usuarioId = Guid.NewGuid();

        cliente.Desativar(usuarioId);

        cliente.Ativo.Should().BeFalse();
        cliente.AlteradoPorId.Should().Be(usuarioId);
        cliente.DataAlteracao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Ativar_Cliente_Inativo()
    {
        var cliente = new Cliente(
            Guid.NewGuid(),
            "João da Silva",
            "12345678901",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            null,
            null,
            false);

        var usuarioId = Guid.NewGuid();

        cliente.Ativar(usuarioId);

        cliente.Ativo.Should().BeTrue();
        cliente.AlteradoPorId.Should().Be(usuarioId);
        cliente.DataAlteracao.Should().NotBeNull();
    }

    private static Cliente CriarCliente()
    {
        return new Cliente(
            "João da Silva",
            "12345678901",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid());
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
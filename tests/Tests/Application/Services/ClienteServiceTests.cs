using Application.DTOs;
using Application.DTOs.Cliente;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using Domain.ValueObjects;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class ClienteServiceTests
{
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly ClienteService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public ClienteServiceTests()
    {
        _clienteRepositoryMock =
            new Mock<IClienteRepository>();

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new ClienteService(
            _clienteRepositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Cliente()
    {
        var cliente = CriarCliente();

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        var resultado =
            await _service.ObterPorIdAsync(cliente.Id);

        resultado.Id.Should().Be(cliente.Id);
        resultado.Nome.Should().Be(cliente.Nome);
        resultado.Documento.Should().Be(cliente.Documento);
        resultado.Email.Should().Be(cliente.Email);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () => await _service.ObterPorIdAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Clientes()
    {
        var clientes = new List<Cliente>
        {
            CriarCliente(),
            CriarCliente(
                nome: "Maria Silva",
                documento: "11222333000181",
                tipoPessoa: TipoPessoa.Juridica)
        };

        _clienteRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(clientes);

        var resultado =
            (await _service.ObterTodosAsync())
            .ToList();

        resultado.Should().HaveCount(2);

        resultado[0].Nome.Should()
            .Be(clientes[0].Nome);

        resultado[1].Nome.Should()
            .Be(clientes[1].Nome);
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Lista_Vazia()
    {
        _clienteRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync([]);

        var resultado =
            await _service.ObterTodosAsync();

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterPorDocumentoAsync_Deve_Retornar_Cliente()
    {
        var cliente = CriarCliente();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(
                    cliente.Documento))
            .ReturnsAsync(cliente);

        var resultado =
            await _service.ObterPorDocumentoAsync(
                cliente.Documento);

        resultado.Id.Should().Be(cliente.Id);
        resultado.Documento.Should()
            .Be(cliente.Documento);
    }

    [Fact]
    public async Task ObterPorDocumentoAsync_Deve_Lancar_Excecao_Quando_Nao_Encontrado()
    {
        const string documento = "52998224725";

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(documento))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service
                    .ObterPorDocumentoAsync(documento);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Cliente não encontrado.");
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Cliente()
    {
        var dto = CriarDto();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(dto.Documento))
            .ReturnsAsync((Cliente?)null);

        Cliente? clienteSalvo = null;

        _clienteRepositoryMock
            .Setup(x => x.AdicionarAsync(
                It.IsAny<Cliente>()))
            .Callback<Cliente>(
                cliente => clienteSalvo = cliente)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Nome.Should().Be("João da Silva");
        resultado.Documento.Should()
            .Be("52998224725");

        clienteSalvo.Should().NotBeNull();

        clienteSalvo!.CriadoPorId.Should()
            .Be(_usuarioId);

        _clienteRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Cliente>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Normalizar_Dados()
    {
        var dto = CriarDto();

        dto.Nome = "  João da Silva  ";
        dto.Documento = "529.982.247-25";
        dto.Telefone = "(15) 99999-0001";
        dto.Email = "  JOAO@EMAIL.COM  ";

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(dto.Documento))
            .ReturnsAsync((Cliente?)null);

        _clienteRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Nome.Should()
            .Be("João da Silva");

        resultado.Documento.Should()
            .Be("52998224725");

        resultado.Telefone.Should()
            .Be("15999990001");

        resultado.Email.Should()
            .Be("joao@email.com");
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Endereco()
    {
        var dto = CriarDtoComEndereco();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(dto.Documento))
            .ReturnsAsync((Cliente?)null);

        _clienteRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Endereco.Should().NotBeNull();

        resultado.Endereco!.Logradouro.Should()
            .Be("Rua das Flores");

        resultado.Endereco.Cidade.Should()
            .Be("Sorocaba");

        resultado.Endereco.Estado.Should()
            .Be("SP");
    }

    [Fact]
    public async Task CriarAsync_Deve_Permitir_Endereco_Nulo()
    {
        var dto = CriarDto();
        dto.Endereco = null;

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(dto.Documento))
            .ReturnsAsync((Cliente?)null);

        _clienteRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Cliente>()))
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Endereco.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Criar_Cliente_Com_Documento_Duplicado()
    {
        var dto = CriarDto();
        var existente = CriarCliente();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorDocumentoAsync(dto.Documento))
            .ReturnsAsync(existente);

        var acao =
            async () => await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe um cliente cadastrado com este documento.");

        _clienteRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Cliente>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Cliente()
    {
        var cliente = CriarCliente();

        var dto = CriarAtualizarDto();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(cliente))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            cliente.Id,
            dto);

        cliente.Nome.Should()
            .Be("Maria da Silva");

        cliente.Telefone.Should()
            .Be("15988887777");

        cliente.Email.Should()
            .Be("maria@email.com");

        cliente.AlteradoPorId.Should()
            .Be(_usuarioId);

        cliente.DataAlteracao.Should()
            .NotBeNull();

        _clienteRepositoryMock.Verify(
            x => x.AtualizarAsync(cliente),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Endereco()
    {
        var cliente = CriarCliente();

        var dto = CriarAtualizarDto();
        dto.Endereco = CriarEnderecoDto();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(cliente))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            cliente.Id,
            dto);

        cliente.Endereco.Should().NotBeNull();

        cliente.Endereco!.Logradouro.Should()
            .Be("Rua das Flores");
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var id = Guid.NewGuid();
        var dto = CriarAtualizarDto();

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Cliente não encontrado.");

        _clienteRepositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<Cliente>()),
            Times.Never);
    }

    [Fact]
    public async Task InativarAsync_Deve_Inativar_Cliente()
    {
        var cliente = CriarCliente();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _clienteRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(cliente))
            .Returns(Task.CompletedTask);

        await _service.InativarAsync(cliente.Id);

        cliente.Ativo.Should().BeFalse();

        cliente.AlteradoPorId.Should()
            .Be(_usuarioId);

        _clienteRepositoryMock.Verify(
            x => x.AtualizarAsync(cliente),
            Times.Once);
    }

    [Fact]
    public async Task InativarAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.InativarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Cliente não encontrado.");

        _clienteRepositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<Cliente>()),
            Times.Never);
    }

    private static Cliente CriarCliente(
        string nome = "João da Silva",
        string documento = "52998224725",
        TipoPessoa tipoPessoa =
            TipoPessoa.Fisica)
    {
        return new Cliente(
            nome,
            documento,
            tipoPessoa,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid());
    }

    private static CriarClienteDto CriarDto()
    {
        return new CriarClienteDto
        {
            Nome = "João da Silva",
            Documento = "52998224725",
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "15999990001",
            Email = "joao@email.com"
        };
    }

    private static CriarClienteDto CriarDtoComEndereco()
    {
        var dto = CriarDto();

        dto.Endereco = CriarEnderecoDto();

        return dto;
    }

    private static AtualizarClienteDto CriarAtualizarDto()
    {
        return new AtualizarClienteDto
        {
            Nome = "Maria da Silva",
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "15988887777",
            Email = "maria@email.com"
        };
    }

    private static EnderecoDto CriarEnderecoDto()
    {
        return new EnderecoDto
        {
            Logradouro = "Rua das Flores",
            Numero = "100",
            Complemento = null,
            Bairro = "Centro",
            Cidade = "Sorocaba",
            Estado = "SP",
            Cep = "18000000"
        };
    }
}
using Application.DTOs.Veiculo;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class VeiculoServiceTests
{
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly VeiculoService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public VeiculoServiceTests()
    {
        _veiculoRepositoryMock =
            new Mock<IVeiculoRepository>();

        _clienteRepositoryMock =
            new Mock<IClienteRepository>();

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new VeiculoService(
            _veiculoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Veiculo()
    {
        var veiculo = CriarVeiculo();

        _veiculoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        var resultado =
            await _service.ObterPorIdAsync(veiculo.Id);

        resultado.Should().NotBeNull();

        resultado!.Id.Should().Be(veiculo.Id);
        resultado.ClienteId.Should().Be(veiculo.ClienteId);
        resultado.Placa.Should().Be(veiculo.Placa);
        resultado.Marca.Should().Be(veiculo.Marca);
        resultado.Modelo.Should().Be(veiculo.Modelo);
        resultado.Cor.Should().Be(veiculo.Cor);
        resultado.Ano.Should().Be(veiculo.Ano);
        resultado.Quilometragem.Should()
            .Be(veiculo.Quilometragem);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _veiculoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Veiculo?)null);

        var resultado =
            await _service.ObterPorIdAsync(id);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Veiculos()
    {
        var veiculos = new List<Veiculo>
        {
            CriarVeiculo(),
            CriarVeiculo(
                placa: "DEF5G78",
                marca: "Chevrolet",
                modelo: "Onix")
        };

        _veiculoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(veiculos);

        var resultado =
            (await _service.ObterTodosAsync())
            .ToList();

        resultado.Should().HaveCount(2);
        resultado[0].Placa.Should().Be("ABC1D23");
        resultado[1].Placa.Should().Be("DEF5G78");
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Lista_Vazia()
    {
        _veiculoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync([]);

        var resultado =
            await _service.ObterTodosAsync();

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterPorClienteIdAsync_Deve_Retornar_Veiculos_Do_Cliente()
    {
        var cliente = CriarCliente();

        var veiculos = new List<Veiculo>
        {
            CriarVeiculo(clienteId: cliente.Id),
            CriarVeiculo(
                clienteId: cliente.Id,
                placa: "DEF5G78")
        };

        _clienteRepositoryMock
            .Setup(x => x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorClienteIdAsync(cliente.Id))
            .ReturnsAsync(veiculos);

        var resultado =
            (await _service.ObterPorClienteIdAsync(
                cliente.Id))
            .ToList();

        resultado.Should().HaveCount(2);

        resultado.Should()
            .OnlyContain(x =>
                x.ClienteId == cliente.Id);
    }

    [Fact]
    public async Task ObterPorClienteIdAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var clienteId = Guid.NewGuid();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(clienteId))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service
                    .ObterPorClienteIdAsync(
                        clienteId);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Cliente não encontrado.");

        _veiculoRepositoryMock.Verify(
            x => x.ObterPorClienteIdAsync(
                It.IsAny<Guid>()),
            Times.Never);
    }

    [Fact]
    public async Task ObterPorPlacaAsync_Deve_Retornar_Veiculo()
    {
        var veiculo = CriarVeiculo();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync(
                    veiculo.Placa))
            .ReturnsAsync(veiculo);

        var resultado =
            await _service.ObterPorPlacaAsync(
                veiculo.Placa);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(veiculo.Id);
        resultado.Placa.Should().Be(veiculo.Placa);
    }

    [Fact]
    public async Task ObterPorPlacaAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        const string placa = "ABC1D23";

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync(placa))
            .ReturnsAsync((Veiculo?)null);

        var resultado =
            await _service.ObterPorPlacaAsync(
                placa);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Veiculo()
    {
        var dto = CriarDto();
        var cliente = CriarCliente(dto.ClienteId);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync("ABC1D23"))
            .ReturnsAsync((Veiculo?)null);

        Veiculo? veiculoSalvo = null;

        _veiculoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Veiculo>()))
            .Callback<Veiculo>(
                veiculo =>
                    veiculoSalvo = veiculo)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Placa.Should().Be("ABC1D23");
        resultado.ClienteId.Should()
            .Be(dto.ClienteId);

        veiculoSalvo.Should().NotBeNull();

        veiculoSalvo!.CriadoPorId.Should()
            .Be(_usuarioId);

        _veiculoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Veiculo>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Normalizar_Placa()
    {
        var dto = CriarDto();

        dto.Placa = " abc-1d23 ";

        var cliente = CriarCliente(
            dto.ClienteId);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync("ABC1D23"))
            .ReturnsAsync((Veiculo?)null);

        _veiculoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Veiculo>()))
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.Placa.Should()
            .Be("ABC1D23");

        _veiculoRepositoryMock.Verify(
            x => x.ObterPorPlacaAsync(
                "ABC1D23"),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var dto = CriarDto();

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Cliente não encontrado.");

        _veiculoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Criar_Placa_Duplicada()
    {
        var dto = CriarDto();

        var cliente =
            CriarCliente(dto.ClienteId);

        var existente =
            CriarVeiculo(
                placa: "ABC1D23");

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync("ABC1D23"))
            .ReturnsAsync(existente);

        var acao =
            async () =>
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe um veículo cadastrado com esta placa.");

        _veiculoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Veiculo()
    {
        var veiculo = CriarVeiculo();

        var dto = CriarAtualizarDto();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync(
                    "DEF5G78"))
            .ReturnsAsync((Veiculo?)null);

        _veiculoRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(veiculo))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            veiculo.Id,
            dto);

        veiculo.Placa.Should().Be("DEF5G78");
        veiculo.Marca.Should().Be("Chevrolet");
        veiculo.Modelo.Should().Be("Onix");
        veiculo.Cor.Should().Be("Preto");
        veiculo.Ano.Should().Be(2022);
        veiculo.Quilometragem.Should().Be(28000);

        veiculo.AlteradoPorId.Should()
            .Be(_usuarioId);

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(veiculo),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Permitir_Mesma_Placa_Do_Proprio_Veiculo()
    {
        var veiculo = CriarVeiculo();

        var dto = CriarAtualizarDto();
        dto.Placa = veiculo.Placa;

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync(
                    veiculo.Placa))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(veiculo))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            veiculo.Id,
            dto);

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(veiculo),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Lancar_Excecao_Quando_Veiculo_Nao_Existe()
    {
        var id = Guid.NewGuid();
        var dto = CriarAtualizarDto();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(id))
            .ReturnsAsync((Veiculo?)null);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Veículo não encontrado.");

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Nao_Deve_Permitir_Placa_De_Outro_Veiculo()
    {
        var veiculo = CriarVeiculo();

        var outroVeiculo =
            CriarVeiculo(
                placa: "DEF5G78");

        var dto = CriarAtualizarDto();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorPlacaAsync(
                    "DEF5G78"))
            .ReturnsAsync(outroVeiculo);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    veiculo.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe outro veículo cadastrado com esta placa.");

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<Veiculo>()),
            Times.Never);
    }

    [Fact]
    public async Task InativarAsync_Deve_Inativar_Veiculo()
    {
        var veiculo = CriarVeiculo();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _veiculoRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(veiculo))
            .Returns(Task.CompletedTask);

        await _service.InativarAsync(
            veiculo.Id);

        veiculo.Ativo.Should().BeFalse();

        veiculo.AlteradoPorId.Should()
            .Be(_usuarioId);

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(veiculo),
            Times.Once);
    }

    [Fact]
    public async Task InativarAsync_Deve_Lancar_Excecao_Quando_Veiculo_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(id))
            .ReturnsAsync((Veiculo?)null);

        var acao =
            async () =>
                await _service.InativarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Veículo não encontrado.");

        _veiculoRepositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<Veiculo>()),
            Times.Never);
    }

    private static Veiculo CriarVeiculo(
        Guid? clienteId = null,
        string placa = "ABC1D23",
        string marca = "Volkswagen",
        string modelo = "Gol")
    {
        return new Veiculo(
            clienteId ?? Guid.NewGuid(),
            placa,
            "9BWZZZ377VT004251",
            marca,
            modelo,
            "Prata",
            2020,
            45000,
            Guid.NewGuid());
    }

    private static Cliente CriarCliente(
        Guid? clienteId = null)
    {
        return new Cliente(
            clienteId ?? Guid.NewGuid(),
            "João da Silva",
            "52998224725",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            null,
            true);
    }

    private static CriarVeiculoDto CriarDto()
    {
        return new CriarVeiculoDto
        {
            ClienteId = Guid.NewGuid(),
            Placa = "ABC1D23",
            Chassi = "9BWZZZ377VT004251",
            Marca = "Volkswagen",
            Modelo = "Gol",
            Cor = "Prata",
            Ano = 2020,
            Quilometragem = 45000
        };
    }

    private static AtualizarVeiculoDto CriarAtualizarDto()
    {
        return new AtualizarVeiculoDto
        {
            Placa = "DEF5G78",
            Chassi = "9BWZZZ377VT004999",
            Marca = "Chevrolet",
            Modelo = "Onix",
            Cor = "Preto",
            Ano = 2022,
            Quilometragem = 28000
        };
    }
}
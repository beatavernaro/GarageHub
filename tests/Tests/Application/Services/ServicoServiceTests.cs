using Application.DTOs.Servico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class ServicoServiceTests
{
    private readonly Mock<IServicoRepository> _repositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly ServicoService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public ServicoServiceTests()
    {
        _repositoryMock = new Mock<IServicoRepository>();
        _currentUserMock = new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new ServicoService(
            _repositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Servico()
    {
        var servico = CriarServico();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        var resultado =
            await _service.ObterPorIdAsync(servico.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(servico.Id);
        resultado.CodigoInterno.Should().Be(servico.CodigoInterno);
        resultado.Nome.Should().Be(servico.Nome);
        resultado.Preco.Should().Be(servico.Preco);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Servico?)null);

        var resultado =
            await _service.ObterPorIdAsync(id);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Servicos()
    {
        var servicos = new List<Servico>
        {
            CriarServico(),
            CriarServico(
                codigo: "SER0002",
                nome: "Alinhamento")
        };

        _repositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(servicos);

        var resultado =
            (await _service.ObterTodosAsync()).ToList();

        resultado.Should().HaveCount(2);
        resultado[0].CodigoInterno.Should().Be("SER0001");
        resultado[1].CodigoInterno.Should().Be("SER0002");
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Lista_Vazia()
    {
        _repositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync([]);

        var resultado =
            await _service.ObterTodosAsync();

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task ObterPorCodigoInternoAsync_Deve_Normalizar_Codigo()
    {
        var servico = CriarServico();

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER0001"))
            .ReturnsAsync(servico);

        var resultado =
            await _service.ObterPorCodigoInternoAsync(
                " ser0001 ");

        resultado.Should().NotBeNull();
        resultado!.CodigoInterno.Should().Be("SER0001");

        _repositoryMock.Verify(
            x => x.ObterPorCodigoInternoAsync("SER0001"),
            Times.Once);
    }

    [Fact]
    public async Task ObterPorCodigoInternoAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER9999"))
            .ReturnsAsync((Servico?)null);

        var resultado =
            await _service.ObterPorCodigoInternoAsync(
                "SER9999");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Servico()
    {
        var dto = CriarDto();

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER0001"))
            .ReturnsAsync((Servico?)null);

        Servico? salvo = null;

        _repositoryMock
            .Setup(x => x.AdicionarAsync(It.IsAny<Servico>()))
            .Callback<Servico>(x => salvo = x)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.CodigoInterno.Should().Be("SER0001");
        resultado.Nome.Should().Be("Troca de Óleo");
        resultado.Preco.Should().Be(100m);

        salvo.Should().NotBeNull();
        salvo!.CriadoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AdicionarAsync(It.IsAny<Servico>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Codigo_Duplicado()
    {
        var dto = CriarDto();
        var existente = CriarServico();

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER0001"))
            .ReturnsAsync(existente);

        var acao =
            async () => await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe um serviço cadastrado com este código interno.");

        _repositoryMock.Verify(
            x => x.AdicionarAsync(It.IsAny<Servico>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Servico()
    {
        var servico = CriarServico();

        var dto = new AtualizarServicoDto
        {
            CodigoInterno = "SER0002",
            Nome = "Alinhamento",
            Descricao = "Alinhamento completo"
        };

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER0002"))
            .ReturnsAsync((Servico?)null);

        _repositoryMock
            .Setup(x => x.AtualizarAsync(servico))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            servico.Id,
            dto);

        servico.CodigoInterno.Should().Be("SER0002");
        servico.Nome.Should().Be("Alinhamento");
        servico.Descricao.Should().Be("Alinhamento completo");
        servico.AlteradoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(servico),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Permitir_Mesmo_Codigo_Do_Proprio_Servico()
    {
        var servico = CriarServico();

        var dto = new AtualizarServicoDto
        {
            CodigoInterno = servico.CodigoInterno,
            Nome = "Troca de Óleo Atualizada",
            Descricao = "Descrição atualizada"
        };

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync(
                servico.CodigoInterno))
            .ReturnsAsync(servico);

        _repositoryMock
            .Setup(x => x.AtualizarAsync(servico))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            servico.Id,
            dto);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(servico),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var id = Guid.NewGuid();

        var dto = new AtualizarServicoDto
        {
            CodigoInterno = "SER0002",
            Nome = "Alinhamento",
            Descricao = null
        };

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.AtualizarAsync(id, dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Serviço não encontrado.");
    }

    [Fact]
    public async Task AtualizarAsync_Nao_Deve_Permitir_Codigo_De_Outro_Servico()
    {
        var servico = CriarServico();

        var outro =
            CriarServico(
                codigo: "SER0002",
                nome: "Alinhamento");

        var dto = new AtualizarServicoDto
        {
            CodigoInterno = "SER0002",
            Nome = "Balanceamento",
            Descricao = null
        };

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        _repositoryMock
            .Setup(x => x.ObterPorCodigoInternoAsync("SER0002"))
            .ReturnsAsync(outro);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    servico.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe outro serviço cadastrado com este código interno.");

        _repositoryMock.Verify(
            x => x.AtualizarAsync(It.IsAny<Servico>()),
            Times.Never);
    }

    [Fact]
    public async Task AlterarPrecoAsync_Deve_Alterar_Preco()
    {
        var servico = CriarServico();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        await _service.AlterarPrecoAsync(
            servico.Id,
            250m);

        servico.Preco.Should().Be(250m);
        servico.AlteradoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(servico),
            Times.Once);
    }

    [Fact]
    public async Task AlterarPrecoAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.AlterarPrecoAsync(
                    id,
                    200m);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Serviço não encontrado.");
    }

    [Fact]
    public async Task InativarAsync_Deve_Inativar_Servico()
    {
        var servico = CriarServico();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        await _service.InativarAsync(servico.Id);

        servico.Ativo.Should().BeFalse();
        servico.AlteradoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(servico),
            Times.Once);
    }

    [Fact]
    public async Task InativarAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.InativarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Serviço não encontrado.");
    }

    [Fact]
    public async Task AtivarAsync_Deve_Ativar_Servico()
    {
        var servico = CriarServico();

        servico.Desativar(Guid.NewGuid());

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        await _service.AtivarAsync(servico.Id);

        servico.Ativo.Should().BeTrue();
        servico.AlteradoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(servico),
            Times.Once);
    }

    [Fact]
    public async Task AtivarAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.AtivarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage("Serviço não encontrado.");
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Formatar_Minutos()
    {
        var dados = new List<TempoMedioServicoDto>
        {
            new()
            {
                ServicoId = Guid.NewGuid(),
                CodigoInterno = "SER0001",
                NomeServico = "Troca de Óleo",
                QuantidadeExecucoes = 3,
                TempoMedioSegundos = 2700
            }
        };

        _repositoryMock
            .Setup(x => x.ObterTemposMediosAsync())
            .ReturnsAsync(dados);

        var resultado =
            (await _service.ObterTempoMedioAsync())
            .Single();

        resultado.TempoMedio.Should().Be("45min");
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Formatar_Horas()
    {
        var dados = new List<TempoMedioServicoDto>
        {
            new()
            {
                ServicoId = Guid.NewGuid(),
                CodigoInterno = "SER0001",
                NomeServico = "Troca de Óleo",
                QuantidadeExecucoes = 2,
                TempoMedioSegundos = 5400
            }
        };

        _repositoryMock
            .Setup(x => x.ObterTemposMediosAsync())
            .ReturnsAsync(dados);

        var resultado =
            (await _service.ObterTempoMedioAsync())
            .Single();

        resultado.TempoMedio.Should().Be("1h 30min");
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Formatar_Dias()
    {
        var dados = new List<TempoMedioServicoDto>
        {
            new()
            {
                ServicoId = Guid.NewGuid(),
                CodigoInterno = "SER0001",
                NomeServico = "Troca de Óleo",
                QuantidadeExecucoes = 2,
                TempoMedioSegundos = 91800
            }
        };

        _repositoryMock
            .Setup(x => x.ObterTemposMediosAsync())
            .ReturnsAsync(dados);

        var resultado =
            (await _service.ObterTempoMedioAsync())
            .Single();

        resultado.TempoMedio.Should().Be("1d 1h 30min");
    }

    private static Servico CriarServico(
        string codigo = "SER0001",
        string nome = "Troca de Óleo")
    {
        return new Servico(
            codigo,
            nome,
            "Descrição",
            100m,
            Guid.NewGuid());
    }

    private static CriarServicoDto CriarDto()
    {
        return new CriarServicoDto
        {
            CodigoInterno = "SER0001",
            Nome = "Troca de Óleo",
            Descricao = "Troca completa",
            Preco = 100m
        };
    }
}
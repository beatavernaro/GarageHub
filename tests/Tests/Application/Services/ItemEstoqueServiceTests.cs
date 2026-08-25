using Application.DTOs.ItemEstoque;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class ItemEstoqueServiceTests
{
    private readonly Mock<IItemEstoqueRepository> _repositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly ItemEstoqueService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public ItemEstoqueServiceTests()
    {
        _repositoryMock =
            new Mock<IItemEstoqueRepository>();

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new ItemEstoqueService(
            _repositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Item()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        var resultado =
            await _service.ObterPorIdAsync(item.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(item.Id);
        resultado.CodigoInterno.Should().Be("PEC0001");
        resultado.Nome.Should().Be("Pastilha de Freio");
        resultado.Preco.Should().Be(150m);
        resultado.Estoque.Should().Be(10);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((ItemEstoque?)null);

        var resultado =
            await _service.ObterPorIdAsync(id);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Itens()
    {
        var itens = new List<ItemEstoque>
        {
            CriarItem(),
            CriarItem(
                codigo: "INS0001",
                nome: "Óleo 5W30",
                tipo: TipoItemEstoque.Insumo)
        };

        _repositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(itens);

        var resultado =
            (await _service.ObterTodosAsync())
            .ToList();

        resultado.Should().HaveCount(2);
        resultado[0].CodigoInterno.Should().Be("PEC0001");
        resultado[1].CodigoInterno.Should().Be("INS0001");
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
    public async Task ObterPorCodigoInternoAsync_Deve_Retornar_Item()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("PEC0001"))
            .ReturnsAsync(item);

        var resultado =
            await _service.ObterPorCodigoInternoAsync(
                "PEC0001");

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(item.Id);
        resultado.CodigoInterno.Should().Be("PEC0001");
    }

    [Fact]
    public async Task ObterPorCodigoInternoAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("PEC9999"))
            .ReturnsAsync((ItemEstoque?)null);

        var resultado =
            await _service.ObterPorCodigoInternoAsync(
                "PEC9999");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Item()
    {
        var dto = CriarDto();

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("PEC0001"))
            .ReturnsAsync((ItemEstoque?)null);

        ItemEstoque? itemSalvo = null;

        _repositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<ItemEstoque>()))
            .Callback<ItemEstoque>(
                item => itemSalvo = item)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.CodigoInterno.Should().Be("PEC0001");
        resultado.Nome.Should().Be("Pastilha de Freio");
        resultado.Preco.Should().Be(150m);
        resultado.Estoque.Should().Be(10);

        itemSalvo.Should().NotBeNull();
        itemSalvo!.CriadoPorId.Should().Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<ItemEstoque>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Normalizar_Codigo()
    {
        var dto = CriarDto();

        dto.CodigoInterno = " pec0001 ";

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("PEC0001"))
            .ReturnsAsync((ItemEstoque?)null);

        _repositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<ItemEstoque>()))
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.CodigoInterno.Should().Be("PEC0001");

        _repositoryMock.Verify(
            x =>
                x.ObterPorCodigoInternoAsync("PEC0001"),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Codigo_Duplicado()
    {
        var dto = CriarDto();
        var existente = CriarItem();

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("PEC0001"))
            .ReturnsAsync(existente);

        var acao =
            async () => await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe um item cadastrado com este código interno.");

        _repositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<ItemEstoque>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Atualizar_Item()
    {
        var item = CriarItem();
        var dto = CriarAtualizarDto();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("INS0002"))
            .ReturnsAsync((ItemEstoque?)null);

        _repositoryMock
            .Setup(x => x.AtualizarAsync(item))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            item.Id,
            dto);

        item.CodigoInterno.Should().Be("INS0002");
        item.Nome.Should().Be("Óleo 5W30");
        item.Tipo.Should().Be(TipoItemEstoque.Insumo);
        item.Preco.Should().Be(50m);

        item.AlteradoPorId.Should()
            .Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Permitir_Mesmo_Codigo_Do_Proprio_Item()
    {
        var item = CriarItem();
        var dto = CriarAtualizarDto();

        dto.CodigoInterno = item.CodigoInterno;

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync(
                    item.CodigoInterno))
            .ReturnsAsync(item);

        _repositoryMock
            .Setup(x => x.AtualizarAsync(item))
            .Returns(Task.CompletedTask);

        await _service.AtualizarAsync(
            item.Id,
            dto);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_Deve_Lancar_Excecao_Quando_Item_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((ItemEstoque?)null);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    id,
                    CriarAtualizarDto());

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Item de estoque não encontrado.");

        _repositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<ItemEstoque>()),
            Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_Nao_Deve_Permitir_Codigo_De_Outro_Item()
    {
        var item = CriarItem();

        var outro =
            CriarItem(
                codigo: "INS0002");

        var dto = CriarAtualizarDto();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        _repositoryMock
            .Setup(x =>
                x.ObterPorCodigoInternoAsync("INS0002"))
            .ReturnsAsync(outro);

        var acao =
            async () =>
                await _service.AtualizarAsync(
                    item.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Já existe outro item cadastrado com este código interno.");

        _repositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<ItemEstoque>()),
            Times.Never);
    }

    [Fact]
    public async Task AdicionarEstoqueAsync_Deve_Adicionar_Estoque()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        await _service.AdicionarEstoqueAsync(
            item.Id,
            5);

        item.Estoque.Should().Be(15);

        item.AlteradoPorId.Should()
            .Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task AdicionarEstoqueAsync_Deve_Lancar_Excecao_Quando_Item_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((ItemEstoque?)null);

        var acao =
            async () =>
                await _service.AdicionarEstoqueAsync(
                    id,
                    5);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Item de estoque não encontrado.");
    }

    [Fact]
    public async Task RemoverEstoqueAsync_Deve_Remover_Estoque()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        await _service.RemoverEstoqueAsync(
            item.Id,
            4);

        item.Estoque.Should().Be(6);

        item.AlteradoPorId.Should()
            .Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task RemoverEstoqueAsync_Deve_Propagar_Erro_De_Estoque_Insuficiente()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        var acao =
            async () =>
                await _service.RemoverEstoqueAsync(
                    item.Id,
                    11);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível remover mais itens do que o disponível em estoque.");

        _repositoryMock.Verify(
            x => x.AtualizarAsync(
                It.IsAny<ItemEstoque>()),
            Times.Never);
    }

    [Fact]
    public async Task AlterarPrecoAsync_Deve_Alterar_Preco()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        await _service.AlterarPrecoAsync(
            item.Id,
            200m);

        item.Preco.Should().Be(200m);

        item.AlteradoPorId.Should()
            .Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task AlterarPrecoAsync_Deve_Lancar_Excecao_Quando_Item_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((ItemEstoque?)null);

        var acao =
            async () =>
                await _service.AlterarPrecoAsync(
                    id,
                    200m);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Item de estoque não encontrado.");
    }

    [Fact]
    public async Task InativarAsync_Deve_Inativar_Item()
    {
        var item = CriarItem();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        await _service.InativarAsync(item.Id);

        item.Ativo.Should().BeFalse();

        item.AlteradoPorId.Should()
            .Be(_usuarioId);

        _repositoryMock.Verify(
            x => x.AtualizarAsync(item),
            Times.Once);
    }

    [Fact]
    public async Task InativarAsync_Deve_Lancar_Excecao_Quando_Item_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _repositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((ItemEstoque?)null);

        var acao =
            async () =>
                await _service.InativarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Item de estoque não encontrado.");
    }

    private static ItemEstoque CriarItem(
        string codigo = "PEC0001",
        string nome = "Pastilha de Freio",
        TipoItemEstoque tipo = TipoItemEstoque.Peca)
    {
        return new ItemEstoque(
            codigo,
            nome,
            tipo,
            150m,
            10,
            Guid.NewGuid(),
            "Pastilha dianteira");
    }

    private static CriarItemEstoqueDto CriarDto()
    {
        return new CriarItemEstoqueDto
        {
            CodigoInterno = "PEC0001",
            Nome = "Pastilha de Freio",
            Descricao = "Pastilha dianteira",
            Tipo = TipoItemEstoque.Peca,
            Preco = 150m,
            Estoque = 10
        };
    }

    private static AtualizarItemEstoqueDto CriarAtualizarDto()
    {
        return new AtualizarItemEstoqueDto
        {
            CodigoInterno = "INS0002",
            Nome = "Óleo 5W30",
            Descricao = "Óleo sintético",
            Tipo = TipoItemEstoque.Insumo,
            Preco = 50m
        };
    }
}
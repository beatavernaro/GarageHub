using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class ItemEstoqueTests
{
    [Fact]
    public void Deve_Criar_Item_Estoque_Com_Dados_Validos()
    {
        var criadoPorId = Guid.NewGuid();

        var item = new ItemEstoque(
            "PEC0001",
            "Pastilha de Freio",
            TipoItemEstoque.Peca,
            150m,
            10,
            criadoPorId,
            "Pastilha dianteira");

        item.Id.Should().NotBeEmpty();
        item.CodigoInterno.Should().Be("PEC0001");
        item.Nome.Should().Be("Pastilha de Freio");
        item.Descricao.Should().Be("Pastilha dianteira");
        item.Tipo.Should().Be(TipoItemEstoque.Peca);
        item.Preco.Should().Be(150m);
        item.Estoque.Should().Be(10);
        item.CriadoPorId.Should().Be(criadoPorId);
        item.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Deve_Normalizar_Dados()
    {
        var item = new ItemEstoque(
            " pec0001 ",
            " Pastilha de Freio ",
            TipoItemEstoque.Peca,
            150m,
            10,
            Guid.NewGuid(),
            " Pastilha dianteira ");

        item.CodigoInterno.Should().Be("PEC0001");
        item.Nome.Should().Be("Pastilha de Freio");
        item.Descricao.Should().Be("Pastilha dianteira");
    }

    [Fact]
    public void Deve_Atualizar_Item()
    {
        var item = CriarItem();

        item.Atualizar(
            "ins0002",
            " Óleo 5W30 ",
            " Óleo sintético ",
            TipoItemEstoque.Insumo,
            Guid.NewGuid());

        item.CodigoInterno.Should().Be("INS0002");
        item.Nome.Should().Be("Óleo 5W30");
        item.Descricao.Should().Be("Óleo sintético");
        item.Tipo.Should().Be(TipoItemEstoque.Insumo);
    }

    [Fact]
    public void Deve_Alterar_Preco()
    {
        var item = CriarItem();

        item.AlterarPreco(200m, Guid.NewGuid());

        item.Preco.Should().Be(200m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Alterar_Preco_Para_Valor_Invalido(decimal preco)
    {
        var item = CriarItem();

        var acao = () => item.AlterarPreco(preco, Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O preço deve ser maior que zero.");
    }

    [Fact]
    public void Deve_Adicionar_Estoque()
    {
        var item = CriarItem();

        item.AdicionarEstoque(5, Guid.NewGuid());

        item.Estoque.Should().Be(15);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Adicionar_Quantidade_Invalida(int quantidade)
    {
        var item = CriarItem();

        var acao = () => item.AdicionarEstoque(quantidade, Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void Deve_Remover_Estoque()
    {
        var item = CriarItem();

        item.RemoverEstoque(4, Guid.NewGuid());

        item.Estoque.Should().Be(6);
    }

    [Fact]
    public void Nao_Deve_Remover_Mais_Do_Que_O_Estoque_Disponivel()
    {
        var item = CriarItem();

        var acao = () => item.RemoverEstoque(11, Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>()
            .WithMessage(
                "Não é possível remover mais itens do que o disponível em estoque.");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Remover_Quantidade_Invalida(int quantidade)
    {
        var item = CriarItem();

        var acao = () => item.RemoverEstoque(quantidade, Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>();
    }

    private static ItemEstoque CriarItem()
    {
        return new ItemEstoque(
            "PEC0001",
            "Pastilha de Freio",
            TipoItemEstoque.Peca,
            150m,
            10,
            Guid.NewGuid());
    }
}
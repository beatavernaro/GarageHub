using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class OrdemServicoItemEstoqueTests
{
    [Fact]
    public void Deve_Criar_Item_Com_Dados_Validos()
    {
        var item = new OrdemServicoItemEstoque(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Óleo 5W30",
            "Óleo sintético",
            4,
            50m,
            Guid.NewGuid());

        item.Quantidade.Should().Be(4);
        item.ValorUnitario.Should().Be(50m);
        item.ValorTotal.Should().Be(200m);
        item.Ativo.Should().BeTrue();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Com_Quantidade_Invalida(
        int quantidade)
    {
        var acao =
            () => new OrdemServicoItemEstoque(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Óleo",
                null,
                quantidade,
                50m,
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Com_Valor_Unitario_Invalido(
        decimal valor)
    {
        var acao =
            () => new OrdemServicoItemEstoque(
                Guid.NewGuid(),
                Guid.NewGuid(),
                "Óleo",
                null,
                1,
                valor,
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }
}
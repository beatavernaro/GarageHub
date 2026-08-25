using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class OrcamentoItemTests
{
    [Fact]
    public void Deve_Criar_Item_De_Servico()
    {
        var item = new OrcamentoItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Troca de Óleo",
            "Troca completa",
            2,
            100m,
            Guid.NewGuid());

        item.ServicoId.Should().NotBeNull();
        item.ItemEstoqueId.Should().BeNull();
        item.Quantidade.Should().Be(2);
        item.ValorUnitario.Should().Be(100m);
        item.ValorTotal.Should().Be(200m);
    }

    [Fact]
    public void Deve_Criar_Item_De_Estoque()
    {
        var item = new OrcamentoItem(
            Guid.NewGuid(),
            null,
            Guid.NewGuid(),
            "Óleo",
            "Óleo 5W30",
            4,
            50m,
            Guid.NewGuid());

        item.ServicoId.Should().BeNull();
        item.ItemEstoqueId.Should().NotBeNull();
        item.ValorTotal.Should().Be(200m);
    }

    [Fact]
    public void Nao_Deve_Criar_Com_Servico_E_Item_Estoque()
    {
        var acao = () => new OrcamentoItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Item",
            null,
            1,
            100m,
            Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Nao_Deve_Criar_Sem_Servico_Nem_Item_Estoque()
    {
        var acao = () => new OrcamentoItem(
            Guid.NewGuid(),
            null,
            null,
            "Item",
            null,
            1,
            100m,
            Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Criar_Com_Quantidade_Invalida(int quantidade)
    {
        var acao = () => new OrcamentoItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Serviço",
            null,
            quantidade,
            100m,
            Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deve_Alterar_Quantidade_E_Recalcular_Total()
    {
        var item = CriarItem();

        item.AlterarQuantidade(3, Guid.NewGuid());

        item.Quantidade.Should().Be(3);
        item.ValorTotal.Should().Be(300m);
    }

    [Fact]
    public void Deve_Alterar_Valor_Unitario_E_Recalcular_Total()
    {
        var item = CriarItem();

        item.AlterarValorUnitario(
            150m,
            Guid.NewGuid());

        item.ValorUnitario.Should().Be(150m);
        item.ValorTotal.Should().Be(150m);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Nao_Deve_Alterar_Quantidade_Para_Valor_Invalido(
        int quantidade)
    {
        var item = CriarItem();

        var acao =
            () => item.AlterarQuantidade(
                quantidade,
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    private static OrcamentoItem CriarItem()
    {
        return new OrcamentoItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Troca de Óleo",
            null,
            1,
            100m,
            Guid.NewGuid());
    }
}
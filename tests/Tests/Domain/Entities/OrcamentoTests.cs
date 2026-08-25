using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class OrcamentoTests
{
    [Fact]
    public void Deve_Criar_Orcamento_Em_Elaboracao()
    {
        var orcamento = CriarOrcamento();

        orcamento.Status.Should().Be(
            StatusOrcamento.EmElaboracao);

        orcamento.Desconto.Should().Be(0);
        orcamento.ValorTotal.Should().Be(0);
        orcamento.Itens.Should().BeEmpty();
    }

    [Fact]
    public void Deve_Adicionar_Item_E_Calcular_Total()
    {
        var orcamento = CriarOrcamento();
        var item = CriarItem(2, 100m);

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        orcamento.Itens.Should().ContainSingle();
        orcamento.ValorTotal.Should().Be(200m);
    }

    [Fact]
    public void Deve_Remover_Item_E_Recalcular_Total()
    {
        var orcamento = CriarOrcamento();
        var item = CriarItem();

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        orcamento.RemoverItem(
            item.Id,
            Guid.NewGuid());

        item.Ativo.Should().BeFalse();
        orcamento.ValorTotal.Should().Be(0);
    }

    [Fact]
    public void Nao_Deve_Remover_Item_Inexistente()
    {
        var orcamento = CriarOrcamento();

        var acao =
            () => orcamento.RemoverItem(
                Guid.NewGuid(),
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deve_Alterar_Quantidade_Do_Item()
    {
        var orcamento = CriarOrcamento();
        var item = CriarItem();

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        orcamento.AlterarQuantidadeItem(
            item.Id,
            3,
            Guid.NewGuid());

        orcamento.ValorTotal.Should().Be(300m);
    }

    [Fact]
    public void Deve_Alterar_Valor_Do_Item()
    {
        var orcamento = CriarOrcamento();
        var item = CriarItem();

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        orcamento.AlterarValorUnitarioItem(
            item.Id,
            200m,
            Guid.NewGuid());

        orcamento.ValorTotal.Should().Be(200m);
    }

    [Fact]
    public void Deve_Aplicar_Desconto()
    {
        var orcamento = CriarOrcamento();

        orcamento.AdicionarItem(
            CriarItem(),
            Guid.NewGuid());

        orcamento.AplicarDesconto(
            20m,
            Guid.NewGuid());

        orcamento.Desconto.Should().Be(20m);
        orcamento.ValorTotal.Should().Be(80m);
    }

    [Fact]
    public void Nao_Deve_Aplicar_Desconto_Negativo()
    {
        var orcamento = CriarOrcamento();

        var acao =
            () => orcamento.AplicarDesconto(
                -1m,
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Nao_Deve_Aplicar_Desconto_Maior_Que_Subtotal()
    {
        var orcamento = CriarOrcamento();

        orcamento.AdicionarItem(
            CriarItem(),
            Guid.NewGuid());

        var acao =
            () => orcamento.AplicarDesconto(
                101m,
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deve_Colocar_Em_Aguardando_Cliente()
    {
        var orcamento = CriarOrcamento();

        orcamento.AdicionarItem(
            CriarItem(),
            Guid.NewGuid());

        orcamento.ColocarEmAguardandoCliente(
            Guid.NewGuid());

        orcamento.Status.Should().Be(
            StatusOrcamento.AguardandoCliente);

        orcamento.DataEnvioCliente.Should().NotBeNull();
    }

    [Fact]
    public void Nao_Deve_Enviar_Orcamento_Sem_Itens()
    {
        var orcamento = CriarOrcamento();

        var acao =
            () => orcamento.ColocarEmAguardandoCliente(
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deve_Aprovar_Orcamento()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        orcamento.Aprovar(Guid.NewGuid());

        orcamento.Status.Should().Be(
            StatusOrcamento.Aprovado);

        orcamento.DataAprovacao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Rejeitar_Orcamento()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        orcamento.Rejeitar(Guid.NewGuid());

        orcamento.Status.Should().Be(
            StatusOrcamento.Rejeitado);

        orcamento.DataRejeicao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Cancelar_Orcamento_Em_Elaboracao()
    {
        var orcamento = CriarOrcamento();

        orcamento.Cancelar(Guid.NewGuid());

        orcamento.Status.Should().Be(
            StatusOrcamento.Cancelado);
    }

    [Fact]
    public void Nao_Deve_Cancelar_Orcamento_Aprovado()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        orcamento.Aprovar(Guid.NewGuid());

        var acao =
            () => orcamento.Cancelar(
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Nao_Deve_Alterar_Item_Quando_Nao_Esta_Em_Elaboracao()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        var acao =
            () => orcamento.AdicionarItem(
                CriarItem(),
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    private static Orcamento CriarOrcamento()
    {
        return new Orcamento(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());
    }

    private static Orcamento CriarOrcamentoAguardandoCliente()
    {
        var orcamento = CriarOrcamento();

        orcamento.AdicionarItem(
            CriarItem(),
            Guid.NewGuid());

        orcamento.ColocarEmAguardandoCliente(
            Guid.NewGuid());

        return orcamento;
    }

    private static OrcamentoItem CriarItem(
        int quantidade = 1,
        decimal valor = 100m)
    {
        return new OrcamentoItem(
            Guid.NewGuid(),
            Guid.NewGuid(),
            null,
            "Troca de Óleo",
            null,
            quantidade,
            valor,
            Guid.NewGuid());
    }
}
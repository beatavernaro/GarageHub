using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class OrdemServicoTests
{
    [Fact]
    public void Deve_Criar_Ordem_Aguardando_Execucao()
    {
        var ordem = CriarOrdem();

        ordem.Status.Should().Be(
            StatusOrdemServico.AguardandoExecucao);

        ordem.DataInicio.Should().BeNull();
        ordem.DataFinalizacao.Should().BeNull();
        ordem.DataEntrega.Should().BeNull();
    }

    [Fact]
    public void Deve_Iniciar_Ordem()
    {
        var ordem = CriarOrdem();
        var usuarioId = Guid.NewGuid();

        ordem.Iniciar(usuarioId);

        ordem.Status.Should().Be(
            StatusOrdemServico.EmExecucao);

        ordem.DataInicio.Should().NotBeNull();
        ordem.AlteradoPorId.Should().Be(usuarioId);
    }

    [Fact]
    public void Nao_Deve_Iniciar_Ordem_Duas_Vezes()
    {
        var ordem = CriarOrdem();

        ordem.Iniciar(Guid.NewGuid());

        var acao =
            () => ordem.Iniciar(Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    [Fact]
    public void Deve_Ficar_Em_Execucao_Quando_Houver_Servico_Em_Execucao()
    {
        var ordem = CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            Guid.NewGuid());

        ordem.AtualizarStatus(
            Guid.NewGuid());

        ordem.Status.Should().Be(
            StatusOrdemServico.EmExecucao);

        ordem.DataInicio.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Finalizar_Quando_Todos_Servicos_Estiverem_Finalizados()
    {
        var ordem = CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            Guid.NewGuid());

        servico.AlterarStatus(
            StatusServico.Finalizada,
            Guid.NewGuid());

        ordem.AtualizarStatus(
            Guid.NewGuid());

        ordem.Status.Should().Be(
            StatusOrdemServico.Finalizada);

        ordem.DataFinalizacao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Entregar_Ordem_Finalizada()
    {
        var ordem = CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            Guid.NewGuid());

        servico.AlterarStatus(
            StatusServico.Finalizada,
            Guid.NewGuid());

        ordem.AtualizarStatus(Guid.NewGuid());

        ordem.Entregar(Guid.NewGuid());

        ordem.Status.Should().Be(
            StatusOrdemServico.Entregue);

        ordem.DataEntrega.Should().NotBeNull();
    }

    [Fact]
    public void Nao_Deve_Entregar_Ordem_Nao_Finalizada()
    {
        var ordem = CriarOrdem();

        var acao =
            () => ordem.Entregar(
                Guid.NewGuid());

        acao.Should().Throw<DomainException>();
    }

    private static OrdemServico CriarOrdem()
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m,
            100m,
            [],
            [],
            Guid.NewGuid());
    }

    private static OrdemServico CriarOrdemComServico()
    {
        var ordemId = Guid.NewGuid();

        var servico =
            new OrdemServicoServico(
                ordemId,
                Guid.NewGuid(),
                "Troca de Óleo",
                null,
                1,
                100m,
                Guid.NewGuid());

        return new OrdemServico(
            ordemId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            0m,
            100m,
            [],
            [servico],
            Guid.NewGuid());
    }
}
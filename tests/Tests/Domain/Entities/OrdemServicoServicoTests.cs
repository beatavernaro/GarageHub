using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class OrdemServicoServicoTests
{
    [Fact]
    public void Deve_Criar_Servico_Aguardando_Execucao()
    {
        var servico = CriarServico();

        servico.Status.Should().Be(
            StatusServico.AguardandoExecucao);

        servico.ValorTotal.Should().Be(100m);
        servico.DataInicio.Should().BeNull();
        servico.DataFinalizacao.Should().BeNull();
    }

    [Fact]
    public void Deve_Iniciar_Servico()
    {
        var servico = CriarServico();
        var usuarioId = Guid.NewGuid();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            usuarioId);

        servico.Status.Should().Be(
            StatusServico.EmExecucao);

        servico.DataInicio.Should().NotBeNull();
        servico.AlteradoPorId.Should().Be(usuarioId);
    }

    [Fact]
    public void Deve_Finalizar_Servico()
    {
        var servico = CriarServico();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            Guid.NewGuid());

        servico.AlterarStatus(
            StatusServico.Finalizada,
            Guid.NewGuid());

        servico.Status.Should().Be(
            StatusServico.Finalizada);

        servico.DataFinalizacao.Should().NotBeNull();
    }

    [Fact]
    public void Nao_Deve_Finalizar_Sem_Iniciar()
    {
        var servico = CriarServico();

        var acao =
            () => servico.AlterarStatus(
                StatusServico.Finalizada,
                Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>()
            .WithMessage(
                "O serviço deve ser iniciado antes de ser finalizado.");
    }

    [Fact]
    public void Nao_Deve_Alterar_Servico_Finalizado()
    {
        var servico = CriarServico();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            Guid.NewGuid());

        servico.AlterarStatus(
            StatusServico.Finalizada,
            Guid.NewGuid());

        var acao =
            () => servico.AlterarStatus(
                StatusServico.EmExecucao,
                Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>();
    }

    [Fact]
    public void Alterar_Para_Mesmo_Status_Nao_Deve_Registrar_Alteracao()
    {
        var servico = CriarServico();

        servico.AlterarStatus(
            StatusServico.AguardandoExecucao,
            Guid.NewGuid());

        servico.DataAlteracao.Should().BeNull();
    }

    private static OrdemServicoServico CriarServico()
    {
        return new OrdemServicoServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "Troca de Óleo",
            "Troca completa",
            1,
            100m,
            Guid.NewGuid());
    }
}
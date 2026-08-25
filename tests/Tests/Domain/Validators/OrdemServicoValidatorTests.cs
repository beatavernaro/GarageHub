using Domain.Entities;
using Domain.Enums;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class OrdemServicoValidatorTests
{
    private readonly OrdemServicoValidator _validator = new();

    [Fact]
    public void Deve_Validar_Ordem_Valida()
    {
        var ordem = CriarOrdem();

        _validator.Validate(ordem)
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Orcamento_Vazio()
    {
        var ordem = CriarOrdem(
            orcamentoId: Guid.Empty);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O orçamento é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Cliente_Vazio()
    {
        var ordem = CriarOrdem(
            clienteId: Guid.Empty);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O cliente é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Veiculo_Vazio()
    {
        var ordem = CriarOrdem(
            veiculoId: Guid.Empty);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O veículo é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Status_Invalido()
    {
        var ordem = CriarOrdem(
            status: (StatusOrdemServico)999);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Status da ordem de serviço inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Sem_Servicos()
    {
        var ordem = CriarOrdem(
            adicionarServico: false);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A ordem de serviço deve possuir pelo menos um serviço.");
    }

    [Fact]
    public void Nao_Deve_Validar_Desconto_Negativo()
    {
        var ordem = CriarOrdem(
            desconto: -1);

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O desconto não pode ser negativo.");
    }

    [Fact]
    public void Nao_Deve_Validar_Finalizacao_Anterior_Ao_Inicio()
    {
        var inicio = DateTime.UtcNow;

        var ordem = CriarOrdem(
            dataInicio: inicio,
            dataFinalizacao: inicio.AddHours(-1));

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A finalização não pode ser anterior ao início.");
    }

    [Fact]
    public void Nao_Deve_Validar_Entrega_Anterior_A_Finalizacao()
    {
        var finalizacao = DateTime.UtcNow;

        var ordem = CriarOrdem(
            dataInicio: finalizacao.AddHours(-1),
            dataFinalizacao: finalizacao,
            dataEntrega: finalizacao.AddMinutes(-30));

        _validator.Validate(ordem).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A entrega não pode ser anterior à finalização.");
    }

    private static OrdemServico CriarOrdem(
        Guid? orcamentoId = null,
        Guid? clienteId = null,
        Guid? veiculoId = null,
        StatusOrdemServico status =
            StatusOrdemServico.AguardandoExecucao,
        decimal desconto = 0,
        DateTime? dataInicio = null,
        DateTime? dataFinalizacao = null,
        DateTime? dataEntrega = null,
        bool adicionarServico = true)
    {
        var ordemId = Guid.NewGuid();

        var servicos =
            adicionarServico
                ? new List<OrdemServicoServico>
                {
                    new(
                        ordemId,
                        Guid.NewGuid(),
                        "Troca de Óleo",
                        null,
                        1,
                        100m,
                        Guid.NewGuid())
                }
                : [];

        return new OrdemServico(
            ordemId,
            orcamentoId ?? Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            veiculoId ?? Guid.NewGuid(),
            status,
            desconto,
            100m,
            dataInicio,
            dataFinalizacao,
            dataEntrega,
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            null,
            true,
            [],
            servicos);
    }
}
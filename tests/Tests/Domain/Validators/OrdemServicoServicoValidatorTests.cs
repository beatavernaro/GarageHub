using Domain.Entities;
using Domain.Enums;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class OrdemServicoServicoValidatorTests
{
    private readonly OrdemServicoServicoValidator _validator = new();

    [Fact]
    public void Deve_Validar_Servico_Valido()
    {
        var servico = CriarServico();

        _validator.Validate(servico)
            .IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Ordem_Vazia()
    {
        var servico = CriarServico(
            ordemServicoId: Guid.Empty);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "A ordem de serviço é obrigatória.");
    }

    [Fact]
    public void Nao_Deve_Validar_ServicoId_Vazio()
    {
        var servico = CriarServico(
            servicoId: Guid.Empty);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O serviço é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Nome_Vazio()
    {
        var servico = CriarServico(nome: "");

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O nome do serviço é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Descricao_Maior_Que_500()
    {
        var servico = CriarServico(
            descricao: new string('A', 501));

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage.Contains(
                    "no máximo 500"));
    }

    [Fact]
    public void Nao_Deve_Validar_Status_Invalido()
    {
        var servico = CriarServico(
            status: (StatusServico)999);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Status do serviço inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Total_Incorreto()
    {
        var servico = CriarServico(
            quantidade: 2,
            valorUnitario: 100,
            valorTotal: 150);

        _validator.Validate(servico).Errors
            .Should()
            .Contain(x =>
                x.ErrorMessage.Contains(
                    "quantidade multiplicada"));
    }

    private static OrdemServicoServico CriarServico(
        Guid? ordemServicoId = null,
        Guid? servicoId = null,
        string nome = "Troca de Óleo",
        string? descricao = null,
        int quantidade = 1,
        decimal valorUnitario = 100,
        decimal valorTotal = 100,
        StatusServico status =
            StatusServico.AguardandoExecucao)
    {
        return new OrdemServicoServico(
            Guid.NewGuid(),
            ordemServicoId ?? Guid.NewGuid(),
            servicoId ?? Guid.NewGuid(),
            nome,
            descricao,
            quantidade,
            valorUnitario,
            valorTotal,
            status,
            null,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            null,
            true);
    }
}
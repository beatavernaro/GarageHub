using Domain.Entities;
using Domain.Enums;
using Domain.Validators;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Validators;

public class OrcamentoValidatorTests
{
    private readonly OrcamentoValidator _validator = new();

    [Fact]
    public void Deve_Validar_Orcamento_Valido()
    {
        var orcamento = CriarOrcamento();

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Nao_Deve_Validar_Cliente_Vazio()
    {
        var orcamento = CriarOrcamento(
            clienteId: Guid.Empty);

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O cliente é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Veiculo_Vazio()
    {
        var orcamento = CriarOrcamento(
            veiculoId: Guid.Empty);

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O veículo é obrigatório.");
    }

    [Fact]
    public void Nao_Deve_Validar_Status_Invalido()
    {
        var orcamento = CriarOrcamento(
            status: (StatusOrcamento)999);

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "Status do orçamento inválido.");
    }

    [Fact]
    public void Nao_Deve_Validar_Sem_Itens()
    {
        var orcamento = CriarOrcamento();

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O orçamento deve possuir pelo menos um item.");
    }

    [Fact]
    public void Nao_Deve_Validar_Desconto_Negativo()
    {
        var orcamento = CriarOrcamento(
            desconto: -1);

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O desconto não pode ser negativo.");
    }

    [Fact]
    public void Nao_Deve_Validar_Desconto_Maior_Que_Subtotal()
    {
        var orcamento = CriarOrcamento(
            desconto: 200);

        orcamento.CarregarItens(
            [CriarItem(orcamento.Id)]);

        var resultado = _validator.Validate(orcamento);

        resultado.Errors.Should()
            .Contain(x =>
                x.ErrorMessage ==
                "O desconto não pode ser maior que o valor dos itens.");
    }

    private static Orcamento CriarOrcamento(
        Guid? clienteId = null,
        Guid? veiculoId = null,
        StatusOrcamento status =
            StatusOrcamento.EmElaboracao,
        decimal desconto = 0)
    {
        return new Orcamento(
            Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            veiculoId ?? Guid.NewGuid(),
            status,
            desconto,
            100m,
            null,
            null,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow,
            null,
            null,
            true);
    }

    private static OrcamentoItem CriarItem(
        Guid orcamentoId)
    {
        return new OrcamentoItem(
            orcamentoId,
            Guid.NewGuid(),
            null,
            "Troca de Óleo",
            null,
            1,
            100m,
            Guid.NewGuid());
    }
}
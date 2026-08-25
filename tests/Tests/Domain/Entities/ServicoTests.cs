using Domain.Entities;
using Domain.Exceptions;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class ServicoTests
{
    [Fact]
    public void Deve_Criar_Servico_Com_Dados_Validos()
    {
        var criadoPorId = Guid.NewGuid();

        var servico = new Servico(
            "SER0001",
            "Troca de Óleo",
            "Troca completa do óleo",
            120m,
            criadoPorId);

        servico.Id.Should().NotBeEmpty();
        servico.CodigoInterno.Should().Be("SER0001");
        servico.Nome.Should().Be("Troca de Óleo");
        servico.Descricao.Should().Be("Troca completa do óleo");
        servico.Preco.Should().Be(120m);
        servico.CriadoPorId.Should().Be(criadoPorId);
        servico.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Deve_Atualizar_E_Normalizar_Servico()
    {
        var servico = CriarServico();
        var usuarioId = Guid.NewGuid();

        servico.Atualizar(
            "SER0002",
            " Alinhamento ",
            " Alinhamento completo ",
            usuarioId);

        servico.CodigoInterno.Should().Be("SER0002");
        servico.Nome.Should().Be("Alinhamento");
        servico.Descricao.Should().Be("Alinhamento completo");
        servico.AlteradoPorId.Should().Be(usuarioId);
        servico.DataAlteracao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Alterar_Preco()
    {
        var servico = CriarServico();
        var usuarioId = Guid.NewGuid();

        servico.AlterarPreco(200m, usuarioId);

        servico.Preco.Should().Be(200m);
        servico.AlteradoPorId.Should().Be(usuarioId);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-10)]
    public void Nao_Deve_Alterar_Preco_Para_Valor_Invalido(decimal preco)
    {
        var servico = CriarServico();

        var acao =
            () => servico.AlterarPreco(
                preco,
                Guid.NewGuid());

        acao.Should()
            .Throw<DomainException>()
            .WithMessage("O preço deve ser maior que zero.");
    }

    private static Servico CriarServico()
    {
        return new Servico(
            "SER0001",
            "Troca de Óleo",
            "Troca completa do óleo",
            120m,
            Guid.NewGuid());
    }
}
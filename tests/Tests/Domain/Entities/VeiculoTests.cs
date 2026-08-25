using Domain.Entities;
using FluentAssertions;

namespace GarageHub.Tests.Domain.Entities;

public class VeiculoTests
{
    [Fact]
    public void Deve_Criar_Veiculo_Com_Dados_Validos()
    {
        var clienteId = Guid.NewGuid();
        var criadoPorId = Guid.NewGuid();

        var veiculo = new Veiculo(
            clienteId,
            "ABC1D23",
            "9BWZZZ377VT004251",
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            criadoPorId);

        veiculo.Id.Should().NotBeEmpty();
        veiculo.ClienteId.Should().Be(clienteId);
        veiculo.Placa.Should().Be("ABC1D23");
        veiculo.Chassi.Should().Be("9BWZZZ377VT004251");
        veiculo.Marca.Should().Be("Volkswagen");
        veiculo.Modelo.Should().Be("Gol");
        veiculo.Cor.Should().Be("Prata");
        veiculo.Ano.Should().Be(2020);
        veiculo.Quilometragem.Should().Be(45000);
        veiculo.CriadoPorId.Should().Be(criadoPorId);
        veiculo.Ativo.Should().BeTrue();
    }

    [Fact]
    public void Deve_Normalizar_Dados_Ao_Criar_Veiculo()
    {
        var veiculo = new Veiculo(
            Guid.NewGuid(),
            " abc-1d23 ",
            " 9bwzzz377vt004251 ",
            " Volkswagen ",
            " Gol ",
            " Prata ",
            2020,
            45000,
            Guid.NewGuid());

        veiculo.Placa.Should().Be("ABC1D23");
        veiculo.Chassi.Should().Be("9BWZZZ377VT004251");
        veiculo.Marca.Should().Be("Volkswagen");
        veiculo.Modelo.Should().Be("Gol");
        veiculo.Cor.Should().Be("Prata");
    }

    [Fact]
    public void Deve_Permitir_Chassi_Nulo()
    {
        var veiculo = new Veiculo(
            Guid.NewGuid(),
            "ABC1D23",
            null,
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            Guid.NewGuid());

        veiculo.Chassi.Should().BeNull();
    }

    [Fact]
    public void Deve_Atualizar_Dados_Do_Veiculo()
    {
        var veiculo = CriarVeiculo();
        var usuarioId = Guid.NewGuid();

        veiculo.Atualizar(
            "DEF5G78",
            "9BWZZZ377VT004999",
            "Chevrolet",
            "Onix",
            "Preto",
            2022,
            28000,
            usuarioId);

        veiculo.Placa.Should().Be("DEF5G78");
        veiculo.Chassi.Should().Be("9BWZZZ377VT004999");
        veiculo.Marca.Should().Be("Chevrolet");
        veiculo.Modelo.Should().Be("Onix");
        veiculo.Cor.Should().Be("Preto");
        veiculo.Ano.Should().Be(2022);
        veiculo.Quilometragem.Should().Be(28000);
    }

    [Fact]
    public void Deve_Normalizar_Dados_Ao_Atualizar_Veiculo()
    {
        var veiculo = CriarVeiculo();

        veiculo.Atualizar(
            " def-5g78 ",
            " 9bwzzz377vt004999 ",
            " Chevrolet ",
            " Onix ",
            " Preto ",
            2022,
            28000,
            Guid.NewGuid());

        veiculo.Placa.Should().Be("DEF5G78");
        veiculo.Chassi.Should().Be("9BWZZZ377VT004999");
        veiculo.Marca.Should().Be("Chevrolet");
        veiculo.Modelo.Should().Be("Onix");
        veiculo.Cor.Should().Be("Preto");
    }

    [Fact]
    public void Deve_Registrar_Usuario_Que_Alterou_Veiculo()
    {
        var veiculo = CriarVeiculo();
        var usuarioId = Guid.NewGuid();

        veiculo.Atualizar(
            "DEF5G78",
            null,
            "Chevrolet",
            "Onix",
            "Preto",
            2022,
            28000,
            usuarioId);

        veiculo.AlteradoPorId.Should().Be(usuarioId);

        veiculo.DataAlteracao.Should().NotBeNull();

        veiculo.DataAlteracao.Should().BeCloseTo(
            DateTime.UtcNow,
            TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Atualizar_Nao_Deve_Alterar_ClienteId()
    {
        var clienteId = Guid.NewGuid();

        var veiculo = new Veiculo(
            clienteId,
            "ABC1D23",
            null,
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            Guid.NewGuid());

        veiculo.Atualizar(
            "DEF5G78",
            null,
            "Chevrolet",
            "Onix",
            "Preto",
            2022,
            28000,
            Guid.NewGuid());

        veiculo.ClienteId.Should().Be(clienteId);
    }

    [Fact]
    public void Deve_Reconstruir_Veiculo_Com_Dados_De_Auditoria()
    {
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var criadoPorId = Guid.NewGuid();
        var alteradoPorId = Guid.NewGuid();

        var dataCriacao =
            DateTime.UtcNow.AddDays(-5);

        var dataAlteracao =
            DateTime.UtcNow.AddDays(-1);

        var veiculo = new Veiculo(
            id,
            clienteId,
            "ABC1D23",
            "9BWZZZ377VT004251",
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            criadoPorId,
            dataCriacao,
            dataAlteracao,
            alteradoPorId,
            false);

        veiculo.Id.Should().Be(id);
        veiculo.ClienteId.Should().Be(clienteId);
        veiculo.CriadoPorId.Should().Be(criadoPorId);
        veiculo.DataCriacao.Should().Be(dataCriacao);
        veiculo.AlteradoPorId.Should().Be(alteradoPorId);
        veiculo.DataAlteracao.Should().Be(dataAlteracao);
        veiculo.Ativo.Should().BeFalse();
    }

    [Fact]
    public void Deve_Desativar_Veiculo()
    {
        var veiculo = CriarVeiculo();
        var usuarioId = Guid.NewGuid();

        veiculo.Desativar(usuarioId);

        veiculo.Ativo.Should().BeFalse();
        veiculo.AlteradoPorId.Should().Be(usuarioId);
        veiculo.DataAlteracao.Should().NotBeNull();
    }

    [Fact]
    public void Deve_Ativar_Veiculo_Inativo()
    {
        var veiculo = new Veiculo(
            Guid.NewGuid(),
            Guid.NewGuid(),
            "ABC1D23",
            null,
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-1),
            null,
            null,
            false);

        var usuarioId = Guid.NewGuid();

        veiculo.Ativar(usuarioId);

        veiculo.Ativo.Should().BeTrue();
        veiculo.AlteradoPorId.Should().Be(usuarioId);
        veiculo.DataAlteracao.Should().NotBeNull();
    }

    private static Veiculo CriarVeiculo()
    {
        return new Veiculo(
            Guid.NewGuid(),
            "ABC1D23",
            "9BWZZZ377VT004251",
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            Guid.NewGuid());
    }
}
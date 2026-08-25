using Application.DTOs.OrdemServico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class OrdemServicoServiceTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IOrcamentoRepository> _orcamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly OrdemServicoService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public OrdemServicoServiceTests()
    {
        _ordemServicoRepositoryMock =
            new Mock<IOrdemServicoRepository>();

        _orcamentoRepositoryMock =
            new Mock<IOrcamentoRepository>();

        _clienteRepositoryMock =
            new Mock<IClienteRepository>();

        _veiculoRepositoryMock =
            new Mock<IVeiculoRepository>();

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new OrdemServicoService(
            _ordemServicoRepositoryMock.Object,
            _orcamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _veiculoRepositoryMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Ordem()
    {
        var ordem = CriarOrdem();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var resultado =
            await _service.ObterPorIdAsync(ordem.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(ordem.Id);
        resultado.OrcamentoId.Should().Be(ordem.OrcamentoId);
        resultado.ClienteId.Should().Be(ordem.ClienteId);
        resultado.VeiculoId.Should().Be(ordem.VeiculoId);
        resultado.Status.Should().Be(ordem.Status);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((OrdemServico?)null);

        var resultado =
            await _service.ObterPorIdAsync(id);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Ordens()
    {
        var ordens = new List<OrdemServico>
        {
            CriarOrdem(),
            CriarOrdem()
        };

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(ordens);

        var resultado =
            (await _service.ObterTodosAsync())
            .ToList();

        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Lista_Vazia()
    {
        _ordemServicoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync([]);

        var resultado =
            await _service.ObterTodosAsync();

        resultado.Should().BeEmpty();
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Ordem_A_Partir_Do_Orcamento()
    {
        var orcamento =
            CriarOrcamentoAprovadoComServicoEItem();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        OrdemServico? ordemSalva = null;

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .Callback<OrdemServico>(
                x => ordemSalva = x)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(
                orcamento.Id);

        ordemSalva.Should().NotBeNull();

        ordemSalva!.OrcamentoId.Should()
            .Be(orcamento.Id);

        ordemSalva.ClienteId.Should()
            .Be(orcamento.ClienteId);

        ordemSalva.VeiculoId.Should()
            .Be(orcamento.VeiculoId);

        ordemSalva.Status.Should()
            .Be(StatusOrdemServico.AguardandoExecucao);

        ordemSalva.Servicos.Should()
            .ContainSingle();

        ordemSalva.Itens.Should()
            .ContainSingle();

        resultado.OrcamentoId.Should()
            .Be(orcamento.Id);

        _ordemServicoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<OrdemServico>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Snapshot_Do_Servico()
    {
        var orcamento =
            CriarOrcamentoAprovadoComServicoEItem();

        var itemServico =
            orcamento.Itens.Single(
                x => x.ServicoId.HasValue);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        OrdemServico? ordemSalva = null;

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .Callback<OrdemServico>(
                x => ordemSalva = x)
            .Returns(Task.CompletedTask);

        await _service.CriarAsync(
            orcamento.Id);

        var servico =
            ordemSalva!.Servicos.Single();

        servico.ServicoId.Should()
            .Be(itemServico.ServicoId!.Value);

        servico.NomeServico.Should()
            .Be(itemServico.NomeItem);

        servico.DescricaoServico.Should()
            .Be(itemServico.DescricaoItem);

        servico.Quantidade.Should()
            .Be(itemServico.Quantidade);

        servico.ValorUnitario.Should()
            .Be(itemServico.ValorUnitario);

        servico.Status.Should()
            .Be(StatusServico.AguardandoExecucao);
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Snapshot_Do_Item_Estoque()
    {
        var orcamento =
            CriarOrcamentoAprovadoComServicoEItem();

        var itemOrcamento =
            orcamento.Itens.Single(
                x => x.ItemEstoqueId.HasValue);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        OrdemServico? ordemSalva = null;

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .Callback<OrdemServico>(
                x => ordemSalva = x)
            .Returns(Task.CompletedTask);

        await _service.CriarAsync(
            orcamento.Id);

        var item =
            ordemSalva!.Itens.Single();

        item.ItemEstoqueId.Should()
            .Be(itemOrcamento.ItemEstoqueId!.Value);

        item.NomeItem.Should()
            .Be(itemOrcamento.NomeItem);

        item.DescricaoItem.Should()
            .Be(itemOrcamento.DescricaoItem);

        item.Quantidade.Should()
            .Be(itemOrcamento.Quantidade);

        item.ValorUnitario.Should()
            .Be(itemOrcamento.ValorUnitario);
    }

    [Fact]
    public async Task CriarAsync_Deve_Lancar_Excecao_Quando_Orcamento_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _orcamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Orcamento?)null);

        var acao =
            async () =>
                await _service.CriarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Orçamento não encontrado.");

        _ordemServicoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<OrdemServico>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Orcamento_Nao_Aprovado()
    {
        var orcamento =
            new Orcamento(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid());

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var acao =
            async () =>
                await _service.CriarAsync(
                    orcamento.Id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Só é possível gerar uma ordem de serviço a partir de um orçamento aprovado.");

        _ordemServicoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<OrdemServico>()),
            Times.Never);
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Deve_Iniciar_Servico()
    {
        var ordem =
            CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        await _service.AlterarStatusServicoAsync(
            ordem.Id,
            servico.Id,
            StatusServico.EmExecucao);

        servico.Status.Should()
            .Be(StatusServico.EmExecucao);

        servico.DataInicio.Should()
            .NotBeNull();

        ordem.Status.Should()
            .Be(StatusOrdemServico.EmExecucao);

        _ordemServicoRepositoryMock.Verify(
            x => x.AtualizarServicoStatusAsync(
                servico),
            Times.Once);

        _ordemServicoRepositoryMock.Verify(
            x => x.AtualizarAsync(ordem),
            Times.Once);
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Deve_Finalizar_Ordem_Quando_Todos_Servicos_Finalizados()
    {
        var ordem =
            CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            _usuarioId);

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        await _service.AlterarStatusServicoAsync(
            ordem.Id,
            servico.Id,
            StatusServico.Finalizada);

        servico.Status.Should()
            .Be(StatusServico.Finalizada);

        ordem.Status.Should()
            .Be(StatusOrdemServico.Finalizada);

        ordem.DataFinalizacao.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Deve_Lancar_Excecao_Quando_Ordem_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((OrdemServico?)null);

        var acao =
            async () =>
                await _service.AlterarStatusServicoAsync(
                    id,
                    Guid.NewGuid(),
                    StatusServico.EmExecucao);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Ordem de serviço não encontrada.");
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var ordem =
            CriarOrdemComServico();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var acao =
            async () =>
                await _service.AlterarStatusServicoAsync(
                    ordem.Id,
                    Guid.NewGuid(),
                    StatusServico.EmExecucao);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Serviço não encontrado na ordem de serviço.");
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Nao_Deve_Alterar_Ordem_Entregue()
    {
        var ordem =
            CriarOrdemEntregue();

        var servico =
            ordem.Servicos.Single();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var acao =
            async () =>
                await _service.AlterarStatusServicoAsync(
                    ordem.Id,
                    servico.Id,
                    StatusServico.EmExecucao);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível alterar serviços de uma ordem já entregue.");
    }

    [Fact]
    public async Task EntregarAsync_Deve_Entregar_Ordem_Finalizada()
    {
        var ordem =
            CriarOrdemFinalizada();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        await _service.EntregarAsync(
            ordem.Id);

        ordem.Status.Should()
            .Be(StatusOrdemServico.Entregue);

        ordem.DataEntrega.Should()
            .NotBeNull();

        _ordemServicoRepositoryMock.Verify(
            x => x.AtualizarAsync(ordem),
            Times.Once);
    }

    [Fact]
    public async Task EntregarAsync_Nao_Deve_Entregar_Ordem_Nao_Finalizada()
    {
        var ordem =
            CriarOrdemComServico();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var acao =
            async () =>
                await _service.EntregarAsync(
                    ordem.Id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "A ordem de serviço não está finalizada.");
    }

    [Fact]
    public async Task ObterAcompanhamentoAsync_Deve_Retornar_Dados_Amigaveis()
    {
        var cliente = CriarCliente();

        var veiculo =
            CriarVeiculo(cliente.Id);

        var ordem =
            CriarOrdemComServico(
                cliente.Id,
                veiculo.Id);

        var servico =
            ordem.Servicos.Single();

        servico.AlterarStatus(
            StatusServico.EmExecucao,
            _usuarioId);

        ordem.AtualizarStatus(_usuarioId);

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterAtualPorPlacaAsync("ABC1D23"))
            .ReturnsAsync(ordem);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        var resultado =
            await _service.ObterAcompanhamentoAsync(
                " abc-1d23 ");

        resultado.Should().NotBeNull();

        resultado!.Cliente.Should()
            .Be("João da Silva");

        resultado.Veiculo.Should()
            .Be("Volkswagen Gol");

        resultado.Placa.Should()
            .Be("ABC1D23");

        resultado.Status.Should()
            .Be("Em execução");

        resultado.Servicos.Should()
            .ContainSingle();

        resultado.Servicos.Single().Status.Should()
            .Be("Em execução");
    }

    [Fact]
    public async Task ObterAcompanhamentoAsync_Deve_Retornar_Null_Quando_Nao_Houver_Ordem()
    {
        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterAtualPorPlacaAsync("ABC1D23"))
            .ReturnsAsync((OrdemServico?)null);

        var resultado =
            await _service.ObterAcompanhamentoAsync(
                "ABC1D23");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterAcompanhamentoAsync_Deve_Retornar_Null_Quando_Cliente_Nao_Existe()
    {
        var ordem =
            CriarOrdemComServico();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterAtualPorPlacaAsync("ABC1D23"))
            .ReturnsAsync(ordem);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.ClienteId))
            .ReturnsAsync((Cliente?)null);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.VeiculoId))
            .ReturnsAsync(
                CriarVeiculo(ordem.ClienteId));

        var resultado =
            await _service.ObterAcompanhamentoAsync(
                "ABC1D23");

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Retornar_Zero_Quando_Nao_Houver_Ordens()
    {
        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterTemposOrdensAsync())
            .ReturnsAsync([]);

        var resultado =
            await _service.ObterTempoMedioAsync();

        resultado.QuantidadeOrdens.Should()
            .Be(0);

        resultado.TempoMedioGeral.Should()
            .Be("0min");

        resultado.Ordens.Should()
            .BeEmpty();
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Calcular_Media()
    {
        var inicio = DateTime.UtcNow;

        var tempos =
            new List<TempoOrdemServicoDto>
            {
                new()
                {
                    OrdemServicoId = Guid.NewGuid(),
                    DataInicio = inicio,
                    DataFinalizacao =
                        inicio.AddHours(1)
                },
                new()
                {
                    OrdemServicoId = Guid.NewGuid(),
                    DataInicio = inicio,
                    DataFinalizacao =
                        inicio.AddHours(2)
                }
            };

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterTemposOrdensAsync())
            .ReturnsAsync(tempos);

        var resultado =
            await _service.ObterTempoMedioAsync();

        resultado.QuantidadeOrdens.Should()
            .Be(2);

        resultado.TempoMedioGeral.Should()
            .Be("1h 30min");

        resultado.Ordens.Should()
            .HaveCount(2);

        resultado.Ordens[0].TempoExecucao.Should()
            .Be("1h 0min");

        resultado.Ordens[1].TempoExecucao.Should()
            .Be("2h 0min");
    }

    [Fact]
    public async Task ObterTempoMedioAsync_Deve_Formatar_Dias()
    {
        var inicio = DateTime.UtcNow;

        var tempos =
            new List<TempoOrdemServicoDto>
            {
                new()
                {
                    OrdemServicoId = Guid.NewGuid(),
                    DataInicio = inicio,
                    DataFinalizacao =
                        inicio
                            .AddDays(1)
                            .AddHours(2)
                            .AddMinutes(30)
                }
            };

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterTemposOrdensAsync())
            .ReturnsAsync(tempos);

        var resultado =
            await _service.ObterTempoMedioAsync();

        resultado.TempoMedioGeral.Should()
            .Be("1d 2h 30min");

        resultado.Ordens.Single()
            .TempoExecucao.Should()
            .Be("1d 2h 30min");
    }

    private static Cliente CriarCliente()
    {
        return new Cliente(
            "João da Silva",
            "52998224725",
            TipoPessoa.Fisica,
            "15999990001",
            "joao@email.com",
            Guid.NewGuid());
    }

    private static Veiculo CriarVeiculo(
        Guid clienteId)
    {
        return new Veiculo(
            clienteId,
            "ABC1D23",
            "9BWZZZ377VT004251",
            "Volkswagen",
            "Gol",
            "Prata",
            2020,
            45000,
            Guid.NewGuid());
    }

    private static Orcamento
        CriarOrcamentoAprovadoComServicoEItem()
    {
        var orcamento =
            new Orcamento(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid());

        var servico =
            new OrcamentoItem(
                orcamento.Id,
                Guid.NewGuid(),
                null,
                "Troca de Óleo",
                "Troca completa",
                1,
                100m,
                Guid.NewGuid());

        var item =
            new OrcamentoItem(
                orcamento.Id,
                null,
                Guid.NewGuid(),
                "Filtro de Óleo",
                "Filtro",
                1,
                50m,
                Guid.NewGuid());

        orcamento.AdicionarItem(
            servico,
            Guid.NewGuid());

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        orcamento.ColocarEmAguardandoCliente(
            Guid.NewGuid());

        orcamento.Aprovar(
            Guid.NewGuid());

        return orcamento;
    }

    private static OrdemServico CriarOrdem(
        Guid? clienteId = null,
        Guid? veiculoId = null)
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            veiculoId ?? Guid.NewGuid(),
            0m,
            100m,
            [],
            [],
            Guid.NewGuid());
    }

    private static OrdemServico CriarOrdemComServico(
        Guid? clienteId = null,
        Guid? veiculoId = null)
    {
        var ordemId = Guid.NewGuid();

        var servico =
            new OrdemServicoServico(
                ordemId,
                Guid.NewGuid(),
                "Troca de Óleo",
                "Troca completa",
                1,
                100m,
                Guid.NewGuid());

        return new OrdemServico(
            ordemId,
            Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            veiculoId ?? Guid.NewGuid(),
            0m,
            100m,
            [],
            [servico],
            Guid.NewGuid());
    }

    private static OrdemServico
        CriarOrdemFinalizada()
    {
        var ordem =
            CriarOrdemComServico();

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

        return ordem;
    }

    private static OrdemServico
        CriarOrdemEntregue()
    {
        var ordem =
            CriarOrdemFinalizada();

        ordem.Entregar(
            Guid.NewGuid());

        return ordem;
    }
}
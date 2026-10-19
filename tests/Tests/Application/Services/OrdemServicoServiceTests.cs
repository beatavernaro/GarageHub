using Application.DTOs.OrdemServico;
using Application.Interfaces;
using Application.Interfaces.Notifications;
using Application.Interfaces.Repositories;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class OrdemServicoServiceTests
{
    private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
    private readonly Mock<IOrcamentoRepository> _orcamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<IServicoRepository> _servicoRepositoryMock;
    private readonly Mock<IItemEstoqueRepository> _itemEstoqueRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICurrentUser> _currentUserMock;
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<ILogger<OrdemServicoService>> _loggerMock;

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

        _servicoRepositoryMock =
            new Mock<IServicoRepository>();

        _itemEstoqueRepositoryMock =
            new Mock<IItemEstoqueRepository>();

        _unitOfWorkMock =
            new Mock<IUnitOfWork>();

        _unitOfWorkMock
            .Setup(x =>
                x.ExecutarEmTransacaoAsync(
                    It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(
                operacao => operacao());

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _emailSenderMock =
            new Mock<IEmailSender>();

        _loggerMock =
            new Mock<ILogger<OrdemServicoService>>();

        _service = new OrdemServicoService(
            _ordemServicoRepositoryMock.Object,
            _orcamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _veiculoRepositoryMock.Object,
            _servicoRepositoryMock.Object,
            _itemEstoqueRepositoryMock.Object,
            _unitOfWorkMock.Object,
            _currentUserMock.Object,
            _emailSenderMock.Object,
            _loggerMock.Object);
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
    public async Task ObterEmAndamentoAsync_Deve_Ordenar_Por_Status_E_Data_De_Criacao()
    {
        var agora = DateTime.UtcNow;

        var aguardandoAntiga =
            CriarOrdemComStatus(StatusOrdemServico.AguardandoExecucao, agora.AddDays(-5));
        var aguardandoRecente =
            CriarOrdemComStatus(StatusOrdemServico.AguardandoExecucao, agora.AddDays(-1));
        var execucaoAntiga =
            CriarOrdemComStatus(StatusOrdemServico.EmExecucao, agora.AddDays(-4));
        var execucaoRecente =
            CriarOrdemComStatus(StatusOrdemServico.EmExecucao, agora.AddDays(-2));

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(
            [
                aguardandoRecente,
                execucaoRecente,
                aguardandoAntiga,
                execucaoAntiga
            ]);

        var resultado =
            (await _service.ObterEmAndamentoAsync())
            .ToList();

        resultado.Select(x => x.Id).Should()
            .Equal(
                execucaoAntiga.Id,
                execucaoRecente.Id,
                aguardandoAntiga.Id,
                aguardandoRecente.Id);
    }

    [Fact]
    public async Task ObterEmAndamentoAsync_Nao_Deve_Retornar_Ordens_Finalizadas_Ou_Entregues()
    {
        var emExecucao =
            CriarOrdemComStatus(StatusOrdemServico.EmExecucao, DateTime.UtcNow);

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(
            [
                CriarOrdemComStatus(StatusOrdemServico.Finalizada, DateTime.UtcNow),
                emExecucao,
                CriarOrdemComStatus(StatusOrdemServico.Entregue, DateTime.UtcNow)
            ]);

        var resultado =
            await _service.ObterEmAndamentoAsync();

        resultado.Should().ContainSingle()
            .Which.Id.Should().Be(emExecucao.Id);
    }

    [Fact]
    public async Task ObterStatusAsync_Deve_Retornar_Status_Da_Ordem()
    {
        var ordem = CriarOrdem();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var resultado =
            await _service.ObterStatusAsync(ordem.Id);

        resultado.Should().NotBeNull();
        resultado!.OrdemServicoId.Should().Be(ordem.Id);
        resultado.Status.Should()
            .Be(StatusOrdemServico.AguardandoExecucao);
        resultado.Descricao.Should()
            .Be("Aguardando execução");
    }

    [Fact]
    public async Task ObterStatusAsync_Deve_Retornar_Status_Atualizado()
    {
        var ordem = CriarOrdemEntregue();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        var resultado =
            await _service.ObterStatusAsync(ordem.Id);

        resultado!.Status.Should()
            .Be(StatusOrdemServico.Entregue);
        resultado.Descricao.Should()
            .Be("Entregue");
    }

    [Fact]
    public async Task ObterStatusAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _ordemServicoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((OrdemServico?)null);

        var resultado =
            await _service.ObterStatusAsync(id);

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
    public async Task AlterarStatusServicoAsync_Deve_Notificar_Cliente_Quando_Status_Da_Ordem_Mudar()
    {
        var cliente = CriarCliente();

        var ordem =
            CriarOrdemComServico(cliente.Id);

        var servico =
            ordem.Servicos.Single();

        ConfigurarOrdemECliente(ordem, cliente);

        await _service.AlterarStatusServicoAsync(
            ordem.Id,
            servico.Id,
            StatusServico.EmExecucao);

        _emailSenderMock.Verify(
            x => x.EnviarAsync(
                cliente.Email,
                It.Is<string>(assunto =>
                    assunto.Contains(ordem.Id.ToString()) &&
                    assunto.Contains("Em execução")),
                It.Is<string>(corpo =>
                    corpo.Contains(ordem.Id.ToString()) &&
                    corpo.Contains(cliente.Nome) &&
                    corpo.Contains("Novo status: Em execução"))),
            Times.Once);
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Nao_Deve_Notificar_Quando_Status_Da_Ordem_Nao_Mudar()
    {
        var cliente = CriarCliente();

        var ordemId = Guid.NewGuid();

        var servicoEmExecucao =
            new OrdemServicoServico(
                ordemId,
                Guid.NewGuid(),
                "Troca de Óleo",
                "Troca completa",
                1,
                100m,
                Guid.NewGuid());

        var servicoAguardando =
            new OrdemServicoServico(
                ordemId,
                Guid.NewGuid(),
                "Alinhamento",
                "Alinhamento completo",
                1,
                80m,
                Guid.NewGuid());

        var ordem =
            new OrdemServico(
                ordemId,
                Guid.NewGuid(),
                cliente.Id,
                Guid.NewGuid(),
                0m,
                180m,
                [],
                [servicoEmExecucao, servicoAguardando],
                Guid.NewGuid());

        servicoEmExecucao.AlterarStatus(
            StatusServico.EmExecucao,
            _usuarioId);

        ordem.AtualizarStatus(_usuarioId);

        ConfigurarOrdemECliente(ordem, cliente);

        // A OS já está em execução: iniciar outro serviço não altera o status dela
        await _service.AlterarStatusServicoAsync(
            ordem.Id,
            servicoAguardando.Id,
            StatusServico.EmExecucao);

        ordem.Status.Should()
            .Be(StatusOrdemServico.EmExecucao);

        _emailSenderMock.Verify(
            x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task EntregarAsync_Deve_Notificar_Cliente_Com_Status_Entregue()
    {
        var cliente = CriarCliente();

        var ordem =
            CriarOrdemFinalizada(cliente.Id);

        ConfigurarOrdemECliente(ordem, cliente);

        await _service.EntregarAsync(ordem.Id);

        _emailSenderMock.Verify(
            x => x.EnviarAsync(
                cliente.Email,
                It.Is<string>(assunto =>
                    assunto.Contains("Entregue")),
                It.Is<string>(corpo =>
                    corpo.Contains(ordem.Id.ToString()) &&
                    corpo.Contains(cliente.Nome) &&
                    corpo.Contains("Novo status: Entregue"))),
            Times.Once);
    }

    [Fact]
    public async Task EntregarAsync_Deve_Atualizar_Ordem_E_Registrar_Log_Quando_Envio_Falhar()
    {
        var cliente = CriarCliente();

        var ordem =
            CriarOrdemFinalizada(cliente.Id);

        ConfigurarOrdemECliente(ordem, cliente);

        _emailSenderMock
            .Setup(x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()))
            .ThrowsAsync(
                new InvalidOperationException(
                    "SMTP indisponível."));

        var acao =
            async () =>
                await _service.EntregarAsync(ordem.Id);

        await acao.Should()
            .NotThrowAsync();

        ordem.Status.Should()
            .Be(StatusOrdemServico.Entregue);

        _ordemServicoRepositoryMock.Verify(
            x => x.AtualizarAsync(ordem),
            Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<InvalidOperationException>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task AlterarStatusServicoAsync_Nao_Deve_Notificar_Quando_Cliente_Nao_Existe()
    {
        var ordem =
            CriarOrdemComServico();

        var servico =
            ordem.Servicos.Single();

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.ClienteId))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.AlterarStatusServicoAsync(
                    ordem.Id,
                    servico.Id,
                    StatusServico.EmExecucao);

        await acao.Should()
            .NotThrowAsync();

        _ordemServicoRepositoryMock.Verify(
            x => x.AtualizarAsync(ordem),
            Times.Once);

        _emailSenderMock.Verify(
            x => x.EnviarAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.IsAny<It.IsAnyType>(),
                It.IsAny<Exception?>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    private void ConfigurarOrdemECliente(
        OrdemServico ordem,
        Cliente cliente)
    {
        _ordemServicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(ordem.Id))
            .ReturnsAsync(ordem);

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);
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

    [Fact]
    public async Task AbrirAsync_Deve_Abrir_Ordem_E_Retornar_Id()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();
        var itemEstoque = CriarItemEstoque();

        ConfigurarAbertura(cliente, veiculo, servico, itemEstoque);

        Orcamento? orcamentoSalvo = null;
        OrdemServico? ordemSalva = null;

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Orcamento>()))
            .Callback<Orcamento>(
                x => orcamentoSalvo = x)
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(
                    It.IsAny<Guid>()))
            .ReturnsAsync(() => orcamentoSalvo);

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .Callback<OrdemServico>(
                x => ordemSalva = x)
            .Returns(Task.CompletedTask);

        var dto = new AbrirOrdemServicoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id,
            Servicos =
            [
                new() { ServicoId = servico.Id, Quantidade = 1, ValorUnitario = 120m }
            ],
            Itens =
            [
                new() { ItemEstoqueId = itemEstoque.Id, Quantidade = 2 }
            ]
        };

        var resultado =
            await _service.AbrirAsync(dto);

        orcamentoSalvo.Should().NotBeNull();
        orcamentoSalvo!.Status.Should()
            .Be(StatusOrcamento.Aprovado);

        ordemSalva.Should().NotBeNull();
        resultado.OrdemServicoId.Should()
            .Be(ordemSalva!.Id);
        resultado.OrcamentoId.Should()
            .Be(orcamentoSalvo.Id);
        resultado.PossuiEstoqueInsuficiente.Should()
            .BeFalse();

        ordemSalva.ClienteId.Should().Be(cliente.Id);
        ordemSalva.VeiculoId.Should().Be(veiculo.Id);
        ordemSalva.CriadoPorId.Should().Be(_usuarioId);
        ordemSalva.ValorTotal.Should().Be(220m);

        ordemSalva.Servicos.Single()
            .ValorUnitario.Should().Be(120m);

        ordemSalva.Itens.Single()
            .ValorUnitario.Should().Be(itemEstoque.Preco);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarItensAsync(
                It.IsAny<Orcamento>()),
            Times.Once);

        _ordemServicoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<OrdemServico>()),
            Times.Once);

        _unitOfWorkMock.Verify(
            x => x.ExecutarEmTransacaoAsync(
                It.IsAny<Func<Task>>()),
            Times.Once);
    }

    [Fact]
    public async Task AbrirAsync_Deve_Gravar_Orcamento_E_Ordem_Na_Mesma_Transacao()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();

        ConfigurarAbertura(cliente, veiculo, servico, null);

        var emTransacao = false;
        var gravacoesForaDaTransacao = 0;
        Orcamento? orcamentoSalvo = null;

        _unitOfWorkMock
            .Setup(x =>
                x.ExecutarEmTransacaoAsync(
                    It.IsAny<Func<Task>>()))
            .Returns<Func<Task>>(async operacao =>
            {
                emTransacao = true;
                await operacao();
                emTransacao = false;
            });

        void RegistrarGravacao()
        {
            if (!emTransacao)
                gravacoesForaDaTransacao++;
        }

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Orcamento>()))
            .Callback<Orcamento>(x =>
            {
                orcamentoSalvo = x;
                RegistrarGravacao();
            })
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AtualizarItensAsync(
                    It.IsAny<Orcamento>()))
            .Callback(RegistrarGravacao)
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AtualizarAsync(
                    It.IsAny<Orcamento>()))
            .Callback(RegistrarGravacao)
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(
                    It.IsAny<Guid>()))
            .ReturnsAsync(() => orcamentoSalvo);

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .Callback(RegistrarGravacao)
            .Returns(Task.CompletedTask);

        await _service.AbrirAsync(
            CriarAbrirOrdemDto(
                cliente.Id,
                veiculo.Id,
                servico.Id));

        gravacoesForaDaTransacao.Should()
            .Be(0);

        _ordemServicoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<OrdemServico>()),
            Times.Once);
    }

    [Fact]
    public async Task AbrirAsync_Deve_Propagar_Falha_Na_Gravacao_Da_Ordem()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();

        ConfigurarAbertura(cliente, veiculo, servico, null);

        Orcamento? orcamentoSalvo = null;

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Orcamento>()))
            .Callback<Orcamento>(
                x => orcamentoSalvo = x)
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(
                    It.IsAny<Guid>()))
            .ReturnsAsync(() => orcamentoSalvo);

        _ordemServicoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<OrdemServico>()))
            .ThrowsAsync(new InvalidOperationException(
                "Falha ao gravar a ordem de serviço."));

        var acao =
            async () =>
                await _service.AbrirAsync(
                    CriarAbrirOrdemDto(
                        cliente.Id,
                        veiculo.Id,
                        servico.Id));

        await acao.Should()
            .ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task AbrirAsync_Deve_Informar_Estoque_Insuficiente()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();
        var itemEstoque = CriarItemEstoque(estoque: 1);

        ConfigurarAbertura(cliente, veiculo, servico, itemEstoque);

        Orcamento? orcamentoSalvo = null;

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Orcamento>()))
            .Callback<Orcamento>(
                x => orcamentoSalvo = x)
            .Returns(Task.CompletedTask);

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(
                    It.IsAny<Guid>()))
            .ReturnsAsync(() => orcamentoSalvo);

        var dto = new AbrirOrdemServicoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id,
            Servicos =
            [
                new() { ServicoId = servico.Id, Quantidade = 1 }
            ],
            Itens =
            [
                new() { ItemEstoqueId = itemEstoque.Id, Quantidade = 3 }
            ]
        };

        var resultado =
            await _service.AbrirAsync(dto);

        resultado.PossuiEstoqueInsuficiente.Should()
            .BeTrue();

        var insuficiente =
            resultado.ItensInsuficientes.Single();

        insuficiente.ItemEstoqueId.Should()
            .Be(itemEstoque.Id);
        insuficiente.QuantidadeFaltante.Should()
            .Be(2);
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Ordem_Sem_Servicos()
    {
        var dto = new AbrirOrdemServicoDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid()
        };

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "A ordem de serviço deve possuir pelo menos um serviço.");

        _orcamentoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Orcamento>()),
            Times.Never);
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Servico_Duplicado()
    {
        var servicoId = Guid.NewGuid();

        var dto = new AbrirOrdemServicoDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid(),
            Servicos =
            [
                new() { ServicoId = servicoId, Quantidade = 1 },
                new() { ServicoId = servicoId, Quantidade = 1 }
            ]
        };

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "O mesmo serviço não pode ser informado mais de uma vez.");
    }

    [Fact]
    public async Task AbrirAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var dto = CriarAbrirOrdemDto(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Cliente não encontrado.");
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Cliente_Inativo()
    {
        var cliente = CriarCliente();

        cliente.Desativar(Guid.NewGuid());

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            Guid.NewGuid(),
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível abrir uma ordem de serviço para um cliente inativo.");
    }

    [Fact]
    public async Task AbrirAsync_Deve_Lancar_Excecao_Quando_Veiculo_Nao_Existe()
    {
        var cliente = CriarCliente();

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            Guid.NewGuid(),
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.VeiculoId))
            .ReturnsAsync((Veiculo?)null);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Veículo não encontrado.");
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Veiculo_Inativo()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);

        veiculo.Desativar(Guid.NewGuid());

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível abrir uma ordem de serviço para um veículo inativo.");
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Veiculo_De_Outro_Cliente()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(Guid.NewGuid());

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "O veículo informado não pertence ao cliente.");
    }

    [Fact]
    public async Task AbrirAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            Guid.NewGuid());

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _servicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Serviço não encontrado.");

        _orcamentoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Orcamento>()),
            Times.Never);
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Servico_Inativo()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();

        servico.Desativar(Guid.NewGuid());

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            servico.Id);

        ConfigurarAbertura(cliente, veiculo, servico, null);

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível abrir uma ordem de serviço com um serviço inativo.");
    }

    [Fact]
    public async Task AbrirAsync_Deve_Lancar_Excecao_Quando_Item_Estoque_Nao_Existe()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();

        ConfigurarAbertura(cliente, veiculo, servico, null);

        _itemEstoqueRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync((ItemEstoque?)null);

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            servico.Id);

        dto.Itens.Add(new()
        {
            ItemEstoqueId = Guid.NewGuid(),
            Quantidade = 1
        });

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Item de estoque não encontrado.");
    }

    [Fact]
    public async Task AbrirAsync_Nao_Deve_Permitir_Item_Estoque_Inativo()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);
        var servico = CriarServico();
        var itemEstoque = CriarItemEstoque();

        itemEstoque.Desativar(Guid.NewGuid());

        ConfigurarAbertura(cliente, veiculo, servico, itemEstoque);

        var dto = CriarAbrirOrdemDto(
            cliente.Id,
            veiculo.Id,
            servico.Id);

        dto.Itens.Add(new()
        {
            ItemEstoqueId = itemEstoque.Id,
            Quantidade = 1
        });

        var acao =
            async () =>
                await _service.AbrirAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível abrir uma ordem de serviço com um item de estoque inativo.");
    }

    private void ConfigurarAbertura(
        Cliente cliente,
        Veiculo veiculo,
        Servico servico,
        ItemEstoque? itemEstoque)
    {
        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        _servicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        if (itemEstoque is not null)
        {
            _itemEstoqueRepositoryMock
                .Setup(x =>
                    x.ObterPorIdAsync(itemEstoque.Id))
                .ReturnsAsync(itemEstoque);
        }
    }

    private static AbrirOrdemServicoDto CriarAbrirOrdemDto(
        Guid clienteId,
        Guid veiculoId,
        Guid servicoId)
    {
        return new AbrirOrdemServicoDto
        {
            ClienteId = clienteId,
            VeiculoId = veiculoId,
            Servicos =
            [
                new() { ServicoId = servicoId, Quantidade = 1 }
            ]
        };
    }

    private static Servico CriarServico()
    {
        return new Servico(
            "SER0001",
            "Troca de Óleo",
            "Troca completa",
            100m,
            Guid.NewGuid());
    }

    private static ItemEstoque CriarItemEstoque(
        int estoque = 10)
    {
        return new ItemEstoque(
            "PEC0001",
            "Filtro de Óleo",
            TipoItemEstoque.Peca,
            50m,
            estoque,
            Guid.NewGuid());
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

    private static OrdemServico CriarOrdemComStatus(
        StatusOrdemServico status,
        DateTime dataCriacao)
    {
        return new OrdemServico(
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            status,
            0m,
            100m,
            null,
            null,
            null,
            Guid.NewGuid(),
            dataCriacao,
            null,
            null,
            true);
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
        CriarOrdemFinalizada(
            Guid? clienteId = null)
    {
        var ordem =
            CriarOrdemComServico(clienteId);

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
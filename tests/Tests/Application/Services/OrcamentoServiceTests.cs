using Application.DTOs.Orcamento;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;
using FluentAssertions;
using Moq;

namespace GarageHub.Tests.Application.Services;

public class OrcamentoServiceTests
{
    private readonly Mock<IOrcamentoRepository> _orcamentoRepositoryMock;
    private readonly Mock<IClienteRepository> _clienteRepositoryMock;
    private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
    private readonly Mock<IServicoRepository> _servicoRepositoryMock;
    private readonly Mock<IItemEstoqueRepository> _itemEstoqueRepositoryMock;
    private readonly Mock<IOrdemServicoService> _ordemServicoServiceMock;
    private readonly Mock<ICurrentUser> _currentUserMock;

    private readonly OrcamentoService _service;

    private readonly Guid _usuarioId = Guid.NewGuid();

    public OrcamentoServiceTests()
    {
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

        _ordemServicoServiceMock =
            new Mock<IOrdemServicoService>();

        _currentUserMock =
            new Mock<ICurrentUser>();

        _currentUserMock
            .Setup(x => x.Id)
            .Returns(_usuarioId);

        _service = new OrcamentoService(
            _orcamentoRepositoryMock.Object,
            _clienteRepositoryMock.Object,
            _veiculoRepositoryMock.Object,
            _servicoRepositoryMock.Object,
            _itemEstoqueRepositoryMock.Object,
            _ordemServicoServiceMock.Object,
            _currentUserMock.Object);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Orcamento()
    {
        var orcamento = CriarOrcamento();

        _orcamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var resultado =
            await _service.ObterPorIdAsync(orcamento.Id);

        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(orcamento.Id);
        resultado.ClienteId.Should().Be(orcamento.ClienteId);
        resultado.VeiculoId.Should().Be(orcamento.VeiculoId);
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Retornar_Null_Quando_Nao_Encontrado()
    {
        var id = Guid.NewGuid();

        _orcamentoRepositoryMock
            .Setup(x => x.ObterPorIdAsync(id))
            .ReturnsAsync((Orcamento?)null);

        var resultado =
            await _service.ObterPorIdAsync(id);

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObterTodosAsync_Deve_Retornar_Orcamentos()
    {
        var orcamentos = new List<Orcamento>
        {
            CriarOrcamento(),
            CriarOrcamento()
        };

        _orcamentoRepositoryMock
            .Setup(x => x.ObterTodosAsync())
            .ReturnsAsync(orcamentos);

        var resultado =
            (await _service.ObterTodosAsync())
            .ToList();

        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObterPorClienteIdAsync_Deve_Retornar_Orcamentos()
    {
        var clienteId = Guid.NewGuid();

        var orcamentos = new List<Orcamento>
        {
            CriarOrcamento(clienteId: clienteId),
            CriarOrcamento(clienteId: clienteId)
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorClienteIdAsync(clienteId))
            .ReturnsAsync(orcamentos);

        var resultado =
            (await _service
                .ObterPorClienteIdAsync(clienteId))
            .ToList();

        resultado.Should().HaveCount(2);

        resultado.Should()
            .OnlyContain(x =>
                x.ClienteId == clienteId);
    }

    [Fact]
    public async Task CriarAsync_Deve_Criar_Orcamento()
    {
        var cliente = CriarCliente();
        var veiculo =
            CriarVeiculo(cliente.Id);

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id
        };

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        _veiculoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(veiculo.Id))
            .ReturnsAsync(veiculo);

        Orcamento? salvo = null;

        _orcamentoRepositoryMock
            .Setup(x =>
                x.AdicionarAsync(
                    It.IsAny<Orcamento>()))
            .Callback<Orcamento>(
                x => salvo = x)
            .Returns(Task.CompletedTask);

        var resultado =
            await _service.CriarAsync(dto);

        resultado.ClienteId.Should()
            .Be(cliente.Id);

        resultado.VeiculoId.Should()
            .Be(veiculo.Id);

        resultado.Status.Should()
            .Be(StatusOrcamento.EmElaboracao);

        salvo.Should().NotBeNull();

        salvo!.CriadoPorId.Should()
            .Be(_usuarioId);

        _orcamentoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Orcamento>()),
            Times.Once);
    }

    [Fact]
    public async Task CriarAsync_Deve_Lancar_Excecao_Quando_Cliente_Nao_Existe()
    {
        var dto = new CriarOrcamentoDto
        {
            ClienteId = Guid.NewGuid(),
            VeiculoId = Guid.NewGuid()
        };

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(dto.ClienteId))
            .ReturnsAsync((Cliente?)null);

        var acao =
            async () =>
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Cliente não encontrado.");

        _orcamentoRepositoryMock.Verify(
            x => x.AdicionarAsync(
                It.IsAny<Orcamento>()),
            Times.Never);
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Cliente_Inativo()
    {
        var cliente = CriarCliente();

        cliente.Desativar(Guid.NewGuid());

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = Guid.NewGuid()
        };

        _clienteRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(cliente.Id))
            .ReturnsAsync(cliente);

        var acao =
            async () =>
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível criar um orçamento para um cliente inativo.");
    }

    [Fact]
    public async Task CriarAsync_Deve_Lancar_Excecao_Quando_Veiculo_Nao_Existe()
    {
        var cliente = CriarCliente();

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = Guid.NewGuid()
        };

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
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Veículo não encontrado.");
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Veiculo_Inativo()
    {
        var cliente = CriarCliente();
        var veiculo = CriarVeiculo(cliente.Id);

        veiculo.Desativar(Guid.NewGuid());

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id
        };

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
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível criar um orçamento para um veículo inativo.");
    }

    [Fact]
    public async Task CriarAsync_Nao_Deve_Permitir_Veiculo_De_Outro_Cliente()
    {
        var cliente = CriarCliente();

        var veiculo =
            CriarVeiculo(Guid.NewGuid());

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id
        };

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
                await _service.CriarAsync(dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "O veículo informado não pertence ao cliente.");
    }

    [Fact]
    public async Task AdicionarItemAsync_Deve_Adicionar_Servico()
    {
        var orcamento = CriarOrcamento();
        var servico = CriarServico();

        var dto = new AdicionarOrcamentoItemDto
        {
            ServicoId = servico.Id,
            ItemEstoqueId = null,
            Quantidade = 1,
            ValorUnitario = 100m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _servicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        await _service.AdicionarItemAsync(
            orcamento.Id,
            dto);

        orcamento.Itens.Should().ContainSingle();

        var item =
            orcamento.Itens.Single();

        item.ServicoId.Should().Be(servico.Id);
        item.NomeItem.Should().Be(servico.Nome);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarItensAsync(orcamento),
            Times.Once);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarAsync(orcamento),
            Times.Once);
    }

    [Fact]
    public async Task AdicionarItemAsync_Deve_Adicionar_Item_Estoque()
    {
        var orcamento = CriarOrcamento();
        var itemEstoque = CriarItemEstoque();

        var dto = new AdicionarOrcamentoItemDto
        {
            ServicoId = null,
            ItemEstoqueId = itemEstoque.Id,
            Quantidade = 2,
            ValorUnitario = 50m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _itemEstoqueRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(itemEstoque.Id))
            .ReturnsAsync(itemEstoque);

        await _service.AdicionarItemAsync(
            orcamento.Id,
            dto);

        orcamento.Itens.Should().ContainSingle();

        var item =
            orcamento.Itens.Single();

        item.ItemEstoqueId.Should()
            .Be(itemEstoque.Id);

        item.NomeItem.Should()
            .Be(itemEstoque.Nome);
    }

    [Fact]
    public async Task AdicionarItemAsync_Deve_Lancar_Excecao_Quando_Servico_Nao_Existe()
    {
        var orcamento = CriarOrcamento();
        var servicoId = Guid.NewGuid();

        var dto = new AdicionarOrcamentoItemDto
        {
            ServicoId = servicoId,
            Quantidade = 1,
            ValorUnitario = 100m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _servicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(servicoId))
            .ReturnsAsync((Servico?)null);

        var acao =
            async () =>
                await _service.AdicionarItemAsync(
                    orcamento.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Serviço não encontrado.");
    }

    [Fact]
    public async Task AdicionarItemAsync_Nao_Deve_Permitir_Servico_Inativo()
    {
        var orcamento = CriarOrcamento();
        var servico = CriarServico();

        servico.Desativar(Guid.NewGuid());

        var dto = new AdicionarOrcamentoItemDto
        {
            ServicoId = servico.Id,
            Quantidade = 1,
            ValorUnitario = 100m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _servicoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(servico.Id))
            .ReturnsAsync(servico);

        var acao =
            async () =>
                await _service.AdicionarItemAsync(
                    orcamento.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível adicionar um serviço inativo ao orçamento.");
    }

    [Fact]
    public async Task AdicionarItemAsync_Nao_Deve_Permitir_Item_Estoque_Inativo()
    {
        var orcamento = CriarOrcamento();
        var item = CriarItemEstoque();

        item.Desativar(Guid.NewGuid());

        var dto = new AdicionarOrcamentoItemDto
        {
            ItemEstoqueId = item.Id,
            Quantidade = 1,
            ValorUnitario = 50m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _itemEstoqueRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(item.Id))
            .ReturnsAsync(item);

        var acao =
            async () =>
                await _service.AdicionarItemAsync(
                    orcamento.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Não é possível adicionar um item de estoque inativo ao orçamento.");
    }

    [Fact]
    public async Task AdicionarItemAsync_Deve_Exigir_Servico_Ou_Item()
    {
        var orcamento = CriarOrcamento();

        var dto = new AdicionarOrcamentoItemDto
        {
            ServicoId = null,
            ItemEstoqueId = null,
            Quantidade = 1,
            ValorUnitario = 100m
        };

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var acao =
            async () =>
                await _service.AdicionarItemAsync(
                    orcamento.Id,
                    dto);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Informe um serviço ou um item de estoque.");
    }

    [Fact]
    public async Task RemoverItemAsync_Deve_Remover_Item()
    {
        var orcamento =
            CriarOrcamentoComItem();

        var item =
            orcamento.Itens.Single();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.RemoverItemAsync(
            orcamento.Id,
            item.Id);

        item.Ativo.Should().BeFalse();

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarItensAsync(orcamento),
            Times.Once);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarAsync(orcamento),
            Times.Once);
    }

    [Fact]
    public async Task AlterarQuantidadeItemAsync_Deve_Alterar_Quantidade()
    {
        var orcamento =
            CriarOrcamentoComItem();

        var item =
            orcamento.Itens.Single();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.AlterarQuantidadeItemAsync(
            orcamento.Id,
            item.Id,
            3);

        item.Quantidade.Should().Be(3);
        item.ValorTotal.Should().Be(300m);
    }

    [Fact]
    public async Task AlterarValorUnitarioItemAsync_Deve_Alterar_Valor()
    {
        var orcamento =
            CriarOrcamentoComItem();

        var item =
            orcamento.Itens.Single();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.AlterarValorUnitarioItemAsync(
            orcamento.Id,
            item.Id,
            200m);

        item.ValorUnitario.Should().Be(200m);
        item.ValorTotal.Should().Be(200m);
    }

    [Fact]
    public async Task AplicarDescontoAsync_Deve_Aplicar_Desconto()
    {
        var orcamento =
            CriarOrcamentoComItem();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.AplicarDescontoAsync(
            orcamento.Id,
            20m);

        orcamento.Desconto.Should().Be(20m);
        orcamento.ValorTotal.Should().Be(80m);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarAsync(orcamento),
            Times.Once);
    }

    [Fact]
    public async Task AlterarStatusAsync_Deve_Colocar_Em_Aguardando_Cliente()
    {
        var orcamento =
            CriarOrcamentoComItem();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.AlterarStatusAsync(
            orcamento.Id,
            StatusOrcamento.AguardandoCliente);

        orcamento.Status.Should()
            .Be(StatusOrcamento.AguardandoCliente);

        orcamento.DataEnvioCliente.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task AlterarStatusAsync_Deve_Cancelar_Orcamento()
    {
        var orcamento = CriarOrcamento();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.AlterarStatusAsync(
            orcamento.Id,
            StatusOrcamento.Cancelado);

        orcamento.Status.Should()
            .Be(StatusOrcamento.Cancelado);
    }

    [Fact]
    public async Task AlterarStatusAsync_Deve_Rejeitar_Status_Que_Possui_Operacao_Propria()
    {
        var orcamento = CriarOrcamento();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var acao =
            async () =>
                await _service.AlterarStatusAsync(
                    orcamento.Id,
                    StatusOrcamento.Aprovado);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Este status deve ser alterado por sua operação específica.");
    }

    [Fact]
    public async Task AprovarAsync_Deve_Aprovar_E_Gerar_Ordem_Servico()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var resultado =
            await _service.AprovarAsync(
                orcamento.Id);

        orcamento.Status.Should()
            .Be(StatusOrcamento.Aprovado);

        orcamento.DataAprovacao.Should()
            .NotBeNull();

        resultado.OrcamentoId.Should()
            .Be(orcamento.Id);

        resultado.Mensagem.Should()
            .Be("Orçamento aprovado.");

        _ordemServicoServiceMock.Verify(
            x => x.CriarAsync(orcamento.Id),
            Times.Once);
    }

    [Fact]
    public async Task AprovarAsync_Deve_Informar_Estoque_Insuficiente()
    {
        var itemEstoque =
            CriarItemEstoque(
                estoque: 1);

        var orcamento =
            CriarOrcamentoComItemEstoque(
                itemEstoque.Id,
                quantidade: 5);

        orcamento.ColocarEmAguardandoCliente(
            Guid.NewGuid());

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        _itemEstoqueRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(itemEstoque.Id))
            .ReturnsAsync(itemEstoque);

        var resultado =
            await _service.AprovarAsync(
                orcamento.Id);

        resultado.ItensInsuficientes
            .Should()
            .ContainSingle();

        var insuficiente =
            resultado.ItensInsuficientes.Single();

        insuficiente.QuantidadeDisponivel
            .Should().Be(1);

        insuficiente.QuantidadeNecessaria
            .Should().Be(5);

        insuficiente.QuantidadeFaltante
            .Should().Be(4);

        resultado.Mensagem.Should()
            .Be(
                "Orçamento aprovado, mas existem itens com estoque insuficiente.");
    }

    [Fact]
    public async Task RejeitarAsync_Deve_Rejeitar_Orcamento()
    {
        var orcamento =
            CriarOrcamentoAguardandoCliente();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.RejeitarAsync(
            orcamento.Id);

        orcamento.Status.Should()
            .Be(StatusOrcamento.Rejeitado);

        orcamento.DataRejeicao.Should()
            .NotBeNull();
    }

    [Fact]
    public async Task ColocarEmAguardandoClienteAsync_Deve_Alterar_Status()
    {
        var orcamento =
            CriarOrcamentoComItem();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service
            .ColocarEmAguardandoClienteAsync(
                orcamento.Id);

        orcamento.Status.Should()
            .Be(StatusOrcamento.AguardandoCliente);
    }

    [Fact]
    public async Task CancelarAsync_Deve_Cancelar()
    {
        var orcamento = CriarOrcamento();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        await _service.CancelarAsync(
            orcamento.Id);

        orcamento.Status.Should()
            .Be(StatusOrcamento.Cancelado);
    }

    [Fact]
    public async Task Operacoes_Devem_Lancar_Excecao_Quando_Orcamento_Nao_Existe()
    {
        var id = Guid.NewGuid();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(id))
            .ReturnsAsync((Orcamento?)null);

        var acao =
            async () =>
                await _service.CancelarAsync(id);

        await acao.Should()
            .ThrowAsync<DomainException>()
            .WithMessage(
                "Orçamento não encontrado.");
    }

    [Fact]
    public async Task ObterPorIdAsync_Deve_Expirar_Orcamento_Vencido()
    {
        var orcamento =
            CriarOrcamentoExpirado();

        _orcamentoRepositoryMock
            .Setup(x =>
                x.ObterPorIdAsync(orcamento.Id))
            .ReturnsAsync(orcamento);

        var resultado =
            await _service.ObterPorIdAsync(
                orcamento.Id);

        resultado!.Status.Should()
            .Be(StatusOrcamento.Expirado);

        _orcamentoRepositoryMock.Verify(
            x => x.AtualizarAsync(orcamento),
            Times.Once);
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

    private static Orcamento CriarOrcamento(
        Guid? clienteId = null,
        Guid? veiculoId = null)
    {
        return new Orcamento(
            clienteId ?? Guid.NewGuid(),
            veiculoId ?? Guid.NewGuid(),
            Guid.NewGuid());
    }

    private static Orcamento CriarOrcamentoComItem()
    {
        var orcamento =
            CriarOrcamento();

        var item =
            new OrcamentoItem(
                orcamento.Id,
                Guid.NewGuid(),
                null,
                "Troca de Óleo",
                null,
                1,
                100m,
                Guid.NewGuid());

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        return orcamento;
    }

    private static Orcamento CriarOrcamentoComItemEstoque(
        Guid itemEstoqueId,
        int quantidade)
    {
        var orcamento =
            CriarOrcamento();

        var item =
            new OrcamentoItem(
                orcamento.Id,
                null,
                itemEstoqueId,
                "Filtro de Óleo",
                null,
                quantidade,
                50m,
                Guid.NewGuid());

        orcamento.AdicionarItem(
            item,
            Guid.NewGuid());

        return orcamento;
    }

    private static Orcamento
        CriarOrcamentoAguardandoCliente()
    {
        var orcamento =
            CriarOrcamentoComItem();

        orcamento.ColocarEmAguardandoCliente(
            Guid.NewGuid());

        return orcamento;
    }

    private static Orcamento
        CriarOrcamentoExpirado()
    {
        var id = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var veiculoId = Guid.NewGuid();

        var orcamento = new Orcamento(
            id,
            clienteId,
            veiculoId,
            StatusOrcamento.AguardandoCliente,
            0m,
            100m,
            DateTime.UtcNow.AddDays(-16),
            null,
            null,
            Guid.NewGuid(),
            DateTime.UtcNow.AddDays(-20),
            null,
            null,
            true);

        var item =
            new OrcamentoItem(
                orcamento.Id,
                Guid.NewGuid(),
                null,
                "Troca de Óleo",
                null,
                1,
                100m,
                Guid.NewGuid());

        orcamento.CarregarItens([item]);

        return orcamento;
    }
}
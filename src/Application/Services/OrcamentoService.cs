using Application.DTOs.Orcamento;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services;

public class OrcamentoService(
    IOrcamentoRepository orcamentoRepository,
    IClienteRepository clienteRepository,
    IVeiculoRepository veiculoRepository,
    IServicoRepository servicoRepository,
    IItemEstoqueRepository itemEstoqueRepository,
    ICurrentUser currentUser) : IOrcamentoService
{
    private readonly IOrcamentoRepository _orcamentoRepository =
        orcamentoRepository;

    private readonly IClienteRepository _clienteRepository =
        clienteRepository;

    private readonly IVeiculoRepository _veiculoRepository =
        veiculoRepository;

    private readonly IServicoRepository _servicoRepository =
        servicoRepository;

    private readonly IItemEstoqueRepository _itemEstoqueRepository =
        itemEstoqueRepository;

    private readonly ICurrentUser _currentUser =
        currentUser;

    public async Task<OrcamentoDto?> ObterPorIdAsync(Guid id)
    {
        var orcamento =
            await _orcamentoRepository.ObterPorIdAsync(id);

        if (orcamento is null)
            return null;

        await VerificarExpiracaoAsync(orcamento);

        return MapearParaDto(orcamento);
    }

    public async Task<IEnumerable<OrcamentoDto>> ObterTodosAsync()
    {
        var orcamentos =
            (await _orcamentoRepository.ObterTodosAsync()).ToList();

        foreach (var orcamento in orcamentos)
            await VerificarExpiracaoAsync(orcamento);

        return orcamentos.Select(MapearParaDto);
    }

    public async Task<IEnumerable<OrcamentoDto>> ObterPorClienteIdAsync(
        Guid clienteId)
    {
        var orcamentos =
            (await _orcamentoRepository
                .ObterPorClienteIdAsync(clienteId))
            .ToList();

        foreach (var orcamento in orcamentos)
            await VerificarExpiracaoAsync(orcamento);

        return orcamentos.Select(MapearParaDto);
    }

    public async Task<OrcamentoDto> CriarAsync(
        CriarOrcamentoDto dto)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(dto.ClienteId)
            ?? throw new DomainException(
                "Cliente não encontrado.");

        if (!cliente.Ativo)
        {
            throw new DomainException(
                "Não é possível criar um orçamento para um cliente inativo.");
        }

        var veiculo =
            await _veiculoRepository.ObterPorIdAsync(dto.VeiculoId)
            ?? throw new DomainException(
                "Veículo não encontrado.");

        if (!veiculo.Ativo)
        {
            throw new DomainException(
                "Não é possível criar um orçamento para um veículo inativo.");
        }

        if (veiculo.ClienteId != dto.ClienteId)
        {
            throw new DomainException(
                "O veículo informado não pertence ao cliente.");
        }

        var orcamento = new Orcamento(
            dto.ClienteId,
            dto.VeiculoId,
            _currentUser.Id);

        await _orcamentoRepository.AdicionarAsync(orcamento);

        return MapearParaDto(orcamento);
    }

    public async Task AdicionarItemAsync(
        Guid id,
        AdicionarOrcamentoItemDto dto)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        ValidarTipoItem(dto.ServicoId, dto.ItemEstoqueId);

        if (dto.ServicoId.HasValue)
        {
            var servico =
                await _servicoRepository.ObterPorIdAsync(
                    dto.ServicoId.Value)
                ?? throw new DomainException(
                    "Serviço não encontrado.");

            if (!servico.Ativo)
            {
                throw new DomainException(
                    "Não é possível adicionar um serviço inativo ao orçamento.");
            }
        }

        if (dto.ItemEstoqueId.HasValue)
        {
            var itemEstoque =
                await _itemEstoqueRepository.ObterPorIdAsync(
                    dto.ItemEstoqueId.Value)
                ?? throw new DomainException(
                    "Item de estoque não encontrado.");

            if (!itemEstoque.Ativo)
            {
                throw new DomainException(
                    "Não é possível adicionar um item de estoque inativo ao orçamento.");
            }
        }

        var item = new OrcamentoItem(
            orcamento.Id,
            dto.ServicoId,
            dto.ItemEstoqueId,
            dto.Quantidade,
            dto.ValorUnitario,
            _currentUser.Id);

        orcamento.AdicionarItem(
            item,
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarItensAsync(orcamento);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task RemoverItemAsync(
        Guid id,
        Guid itemId)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.RemoverItem(
            itemId,
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarItensAsync(orcamento);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task AlterarQuantidadeItemAsync(
        Guid id,
        Guid itemId,
        int quantidade)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.AlterarQuantidadeItem(
            itemId,
            quantidade,
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarItensAsync(orcamento);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task AlterarValorUnitarioItemAsync(
        Guid id,
        Guid itemId,
        decimal valorUnitario)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.AlterarValorUnitarioItem(
            itemId,
            valorUnitario,
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarItensAsync(orcamento);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task AplicarDescontoAsync(
        Guid id,
        decimal desconto)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.AplicarDesconto(
            desconto,
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task AlterarStatusAsync(
        Guid id,
        StatusOrcamento status)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        switch (status)
        {
            case StatusOrcamento.AguardandoCliente:
                orcamento.ColocarEmAguardandoCliente(
                    _currentUser.Id);
                break;

            case StatusOrcamento.Cancelado:
                orcamento.Cancelar(
                    _currentUser.Id);
                break;

            default:
                throw new DomainException(
                    "Este status deve ser alterado por sua operação específica.");
        }

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task<ResultadoAprovacaoOrcamentoDto> AprovarAsync(
        Guid id)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        var itensInsuficientes =
            await VerificarEstoqueInsuficienteAsync(orcamento);

        orcamento.Aprovar(_currentUser.Id);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);

        return new ResultadoAprovacaoOrcamentoDto
        {
            OrcamentoId = orcamento.Id,
            Mensagem = itensInsuficientes.Count == 0
                ? "Orçamento aprovado. O estoque possui quantidade suficiente."
                : "Orçamento aprovado, mas existem itens com estoque insuficiente.",
            ItensInsuficientes = itensInsuficientes
        };
    }

    public async Task RejeitarAsync(Guid id)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.Rejeitar(_currentUser.Id);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task ColocarEmAguardandoClienteAsync(Guid id)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.ColocarEmAguardandoCliente(
            _currentUser.Id);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    public async Task CancelarAsync(Guid id)
    {
        var orcamento =
            await ObterOrcamentoAsync(id);

        orcamento.Cancelar(_currentUser.Id);

        await _orcamentoRepository
            .AtualizarAsync(orcamento);
    }

    private async Task<Orcamento> ObterOrcamentoAsync(Guid id)
    {
        var orcamento =
            await _orcamentoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(
                "Orçamento não encontrado.");

        await VerificarExpiracaoAsync(orcamento);

        return orcamento;
    }

    private async Task VerificarExpiracaoAsync(
        Orcamento orcamento)
    {
        var statusAnterior = orcamento.Status;

        orcamento.VerificarExpiracao(_currentUser.Id);

        if (statusAnterior != orcamento.Status)
        {
            await _orcamentoRepository
                .AtualizarAsync(orcamento);
        }
    }

    private async Task<List<ItemEstoqueInsuficienteDto>>
        VerificarEstoqueInsuficienteAsync(
            Orcamento orcamento)
    {
        var quantidadesNecessarias =
            new Dictionary<Guid, int>();

        foreach (var item in orcamento.Itens.Where(x => x.Ativo))
        {
            if (item.ItemEstoqueId.HasValue)
            {
                AdicionarQuantidadeNecessaria(
                    quantidadesNecessarias,
                    item.ItemEstoqueId.Value,
                    item.Quantidade);

                continue;
            }

            if (!item.ServicoId.HasValue)
                continue;

            var servico =
                await _servicoRepository.ObterPorIdAsync(
                    item.ServicoId.Value);

            if (servico is null)
                continue;

            foreach (var itemServico in servico.ItensEstoque
                         .Where(x => x.Ativo))
            {
                var quantidadeNecessaria =
                    item.Quantidade *
                    itemServico.Quantidade;

                AdicionarQuantidadeNecessaria(
                    quantidadesNecessarias,
                    itemServico.ItemEstoqueId,
                    quantidadeNecessaria);
            }
        }

        var resultado =
            new List<ItemEstoqueInsuficienteDto>();

        foreach (var quantidade in quantidadesNecessarias)
        {
            var itemEstoque =
                await _itemEstoqueRepository
                    .ObterPorIdAsync(quantidade.Key);

            if (itemEstoque is null)
                continue;

            if (itemEstoque.Estoque >= quantidade.Value)
                continue;

            resultado.Add(
                new ItemEstoqueInsuficienteDto
                {
                    ItemEstoqueId = itemEstoque.Id,
                    Nome = itemEstoque.Nome,
                    QuantidadeDisponivel = itemEstoque.Estoque,
                    QuantidadeNecessaria = quantidade.Value,
                    QuantidadeFaltante =
                        quantidade.Value - itemEstoque.Estoque
                });
        }

        return resultado;
    }

    private static void AdicionarQuantidadeNecessaria(
        Dictionary<Guid, int> quantidades,
        Guid itemEstoqueId,
        int quantidade)
    {
        if (quantidades.TryGetValue(
            itemEstoqueId,
            out var quantidadeAtual))
        {
            quantidades[itemEstoqueId] =
                quantidadeAtual + quantidade;

            return;
        }

        quantidades[itemEstoqueId] = quantidade;
    }

    private static void ValidarTipoItem(
        Guid? servicoId,
        Guid? itemEstoqueId)
    {
        if (servicoId.HasValue == itemEstoqueId.HasValue)
        {
            throw new DomainException(
                "Informe um serviço ou um item de estoque, mas não ambos.");
        }
    }

    private static OrcamentoDto MapearParaDto(
        Orcamento orcamento)
    {
        return new OrcamentoDto
        {
            Id = orcamento.Id,
            ClienteId = orcamento.ClienteId,
            VeiculoId = orcamento.VeiculoId,
            Status = orcamento.Status,
            Desconto = orcamento.Desconto,
            ValorTotal = orcamento.ValorTotal,
            DataEnvioCliente = orcamento.DataEnvioCliente,
            DataAprovacao = orcamento.DataAprovacao,
            DataRejeicao = orcamento.DataRejeicao,
            Ativo = orcamento.Ativo,

            Itens = orcamento.Itens
                .Where(x => x.Ativo)
                .Select(x => new OrcamentoItemDto
                {
                    Id = x.Id,
                    ServicoId = x.ServicoId,
                    ItemEstoqueId = x.ItemEstoqueId,
                    Quantidade = x.Quantidade,
                    ValorUnitario = x.ValorUnitario,
                    ValorTotal = x.ValorTotal
                })
                .ToList()
        };
    }
}
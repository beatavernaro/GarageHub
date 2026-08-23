using Application.DTOs.OrdemServico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Exceptions;

namespace Application.Services;

public class OrdemServicoService(
    IOrdemServicoRepository ordemServicoRepository,
    IOrcamentoRepository orcamentoRepository,
    IServicoRepository servicoRepository,
    IItemEstoqueRepository itemEstoqueRepository,
    ICurrentUser currentUser) : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository =
        ordemServicoRepository;

    private readonly IOrcamentoRepository _orcamentoRepository =
        orcamentoRepository;

    private readonly IServicoRepository _servicoRepository =
        servicoRepository;

    private readonly IItemEstoqueRepository _itemEstoqueRepository =
        itemEstoqueRepository;

    private readonly ICurrentUser _currentUser =
        currentUser;

    public async Task<OrdemServicoDto> CriarAsync(
        Guid orcamentoId)
    {
        var orcamento =
            await _orcamentoRepository.ObterPorIdAsync(orcamentoId)
            ?? throw new DomainException(
                "Orçamento não encontrado.");

        if (!orcamento.Ativo)
        {
            throw new DomainException(
                "Não é possível criar uma ordem de serviço para um orçamento inativo.");
        }

        if (orcamento.Status != StatusOrcamento.Aprovado)
        {
            throw new DomainException(
                "Só é possível criar uma ordem de serviço para um orçamento aprovado.");
        }

        var ordemServicoId = Guid.NewGuid();

        var itens = new List<OrdemServicoItemEstoque>();
        var servicos = new List<OrdemServicoServico>();

        foreach (var item in orcamento.Itens.Where(x => x.Ativo))
        {
            if (item.ServicoId.HasValue)
            {
                var servico =
                    await _servicoRepository.ObterPorIdAsync(
                        item.ServicoId.Value)
                    ?? throw new DomainException(
                        "Serviço do orçamento não encontrado.");

                if (!servico.Ativo)
                {
                    throw new DomainException(
                        "Não é possível adicionar um serviço inativo à ordem de serviço.");
                }

                servicos.Add(
                    new OrdemServicoServico(
                        ordemServicoId,
                        item.ServicoId.Value,
                        servico.Nome,
                        servico.Descricao,
                        item.Quantidade,
                        item.ValorUnitario,
                        _currentUser.Id));
            }

            if (item.ItemEstoqueId.HasValue)
            {
                var itemEstoque =
                    await _itemEstoqueRepository.ObterPorIdAsync(
                        item.ItemEstoqueId.Value)
                    ?? throw new DomainException(
                        "Item de estoque do orçamento não encontrado.");

                if (!itemEstoque.Ativo)
                {
                    throw new DomainException(
                        "Não é possível adicionar um item de estoque inativo à ordem de serviço.");
                }

                itens.Add(
                    new OrdemServicoItemEstoque(
                        ordemServicoId,
                        item.ItemEstoqueId.Value,
                        itemEstoque.Nome,
                        itemEstoque.Descricao,
                        item.Quantidade,
                        item.ValorUnitario,
                        _currentUser.Id));
            }
        }

        var ordemServico = new OrdemServico(
            ordemServicoId,
            orcamento.Id,
            orcamento.ClienteId,
            orcamento.VeiculoId,
            orcamento.Desconto,
            orcamento.ValorTotal,
            itens,
            servicos,
            _currentUser.Id);

        await _ordemServicoRepository.AdicionarAsync(
            ordemServico);

        return MapearParaDto(ordemServico);
    }

    public async Task<OrdemServicoDto?> ObterPorIdAsync(
        Guid id)
    {
        var ordemServico =
            await _ordemServicoRepository.ObterPorIdAsync(id);

        if (ordemServico is null)
            return null;

        return MapearParaDto(ordemServico);
    }

    public async Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync()
    {
        var ordens =
            await _ordemServicoRepository.ObterTodosAsync();

        return ordens.Select(MapearParaDto);
    }

    public async Task<IEnumerable<OrdemServicoDto>>
        ObterPorOrcamentoIdAsync(Guid orcamentoId)
    {
        var ordens =
            await _ordemServicoRepository
                .ObterPorOrcamentoIdAsync(orcamentoId);

        return ordens.Select(MapearParaDto);
    }

    public async Task IniciarAsync(Guid id)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(id);

        ordemServico.Iniciar(_currentUser.Id);

        await _ordemServicoRepository
            .AtualizarAsync(ordemServico);
    }

    public async Task AtualizarStatusAsync(Guid id)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(id);

        ordemServico.AtualizarStatus(_currentUser.Id);

        await _ordemServicoRepository
            .AtualizarAsync(ordemServico);
    }

    public async Task EntregarAsync(Guid id)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(id);

        ordemServico.Entregar(_currentUser.Id);

        await _ordemServicoRepository
            .AtualizarAsync(ordemServico);
    }

    public async Task AlterarStatusServicoAsync(
        Guid ordemServicoId,
        Guid servicoId,
        StatusServico status)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(ordemServicoId);

        var servico =
            ordemServico.Servicos
                .FirstOrDefault(x =>
                    x.Id == servicoId);

        if (servico is null)
        {
            throw new DomainException(
                "Serviço não encontrado na ordem de serviço.");
        }

        servico.AlterarStatus(
            status,
            _currentUser.Id);

        ordemServico.AtualizarStatus(
            _currentUser.Id);

        await _ordemServicoRepository
            .AtualizarServicoAsync(servico);

        await _ordemServicoRepository
            .AtualizarAsync(ordemServico);
    }

    private async Task<OrdemServico> ObterOrdemServicoAsync(
        Guid id)
    {
        return await _ordemServicoRepository
            .ObterPorIdAsync(id)
            ?? throw new DomainException(
                "Ordem de serviço não encontrada.");
    }

    private static OrdemServicoDto MapearParaDto(
        OrdemServico ordemServico)
    {
        return new OrdemServicoDto
        {
            Id = ordemServico.Id,
            OrcamentoId = ordemServico.OrcamentoId,
            ClienteId = ordemServico.ClienteId,
            VeiculoId = ordemServico.VeiculoId,
            Status = ordemServico.Status,
            Desconto = ordemServico.Desconto,
            ValorTotal = ordemServico.ValorTotal,
            DataInicio = ordemServico.DataInicio,
            DataFinalizacao = ordemServico.DataFinalizacao,
            DataEntrega = ordemServico.DataEntrega,
            Ativo = ordemServico.Ativo,

            Itens = ordemServico.Itens
                .Where(x => x.Ativo)
                .Select(x => new OrdemServicoItemEstoqueDto
                {
                    Id = x.Id,
                    ItemEstoqueId = x.ItemEstoqueId,
                    NomeItem = x.NomeItem,
                    DescricaoItem = x.DescricaoItem,
                    Quantidade = x.Quantidade,
                    ValorUnitario = x.ValorUnitario,
                    ValorTotal = x.ValorTotal
                })
                .ToList(),

            Servicos = [.. ordemServico.Servicos
                .Where(x => x.Ativo)
                .Select(x => new OrdemServicoServicoDto
                {
                    Id = x.Id,
                    ServicoId = x.ServicoId,
                    NomeServico = x.NomeServico,
                    DescricaoServico = x.DescricaoServico,
                    Quantidade = x.Quantidade,
                    ValorUnitario = x.ValorUnitario,
                    ValorTotal = x.ValorTotal,
                    Status = x.Status
                })]
        };
    }
}
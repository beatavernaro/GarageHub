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
    ICurrentUser currentUser)
    : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository =
        ordemServicoRepository;

    private readonly IOrcamentoRepository _orcamentoRepository =
        orcamentoRepository;

    private readonly ICurrentUser _currentUser =
        currentUser;

    public async Task<OrdemServicoDto?> ObterPorIdAsync(Guid id)
    {
        var ordemServico =
            await _ordemServicoRepository.ObterPorIdAsync(id);

        return ordemServico is null
            ? null
            : MapearParaDto(ordemServico);
    }

    public async Task<IEnumerable<OrdemServicoDto>> ObterTodosAsync()
    {
        var ordens =
            await _ordemServicoRepository.ObterTodosAsync();

        return ordens.Select(MapearParaDto);
    }

    public async Task<OrdemServicoDto> CriarAsync(
        Guid orcamentoId)
    {
        var orcamento =
            await _orcamentoRepository.ObterPorIdAsync(orcamentoId)
            ?? throw new DomainException(
                "Orçamento não encontrado.");

        if (orcamento.Status != StatusOrcamento.Aprovado)
        {
            throw new DomainException(
                "Só é possível gerar uma ordem de serviço a partir de um orçamento aprovado.");
        }

        var ordemServicoId = Guid.NewGuid();

        var servicos = orcamento.Itens
            .Where(x =>
                x.Ativo &&
                x.ServicoId.HasValue)
            .Select(x =>
                new OrdemServicoServico(
                    ordemServicoId,
                    x.ServicoId!.Value,
                    x.NomeItem,
                    x.DescricaoItem,
                    x.Quantidade,
                    x.ValorUnitario,
                    _currentUser.Id))
            .ToList();

        var itens = orcamento.Itens
            .Where(x =>
                x.Ativo &&
                x.ItemEstoqueId.HasValue)
            .Select(x =>
                new OrdemServicoItemEstoque(
                    ordemServicoId,
                    x.ItemEstoqueId!.Value,
                    x.NomeItem,
                    x.DescricaoItem,
                    x.Quantidade,
                    x.ValorUnitario,
                    _currentUser.Id))
            .ToList();

        var ordemServico =
            new OrdemServico(
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

    public async Task AlterarStatusServicoAsync(
        Guid ordemServicoId,
        Guid ordemServicoServicoId,
        StatusServico status)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(ordemServicoId);

        if (ordemServico.Status == StatusOrdemServico.Entregue)
        {
            throw new DomainException(
                "Não é possível alterar serviços de uma ordem já entregue.");
        }

        var servico =
            ordemServico.Servicos.FirstOrDefault(x =>
                x.Id == ordemServicoServicoId &&
                x.Ativo)
            ?? throw new DomainException(
                "Serviço não encontrado na ordem de serviço.");

        servico.AlterarStatus(
            status,
            _currentUser.Id);

        ordemServico.AtualizarStatus(
            _currentUser.Id);

        await _ordemServicoRepository
            .AtualizarServicoStatusAsync(
                ordemServico.Id,
                servico.Id,
                servico.Status,
                servico.DataAlteracao!.Value,
                _currentUser.Id);

        await _ordemServicoRepository
            .AtualizarAsync(ordemServico);
    }

    public async Task EntregarAsync(Guid id)
    {
        var ordemServico =
            await ObterOrdemServicoAsync(id);

        ordemServico.Entregar(
            _currentUser.Id);

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

            Servicos = ordemServico.Servicos
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
                })
                .ToList(),

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
                .ToList()
        };
    }
}
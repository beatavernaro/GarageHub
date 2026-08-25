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
    IClienteRepository clienteRepository,
    IVeiculoRepository veiculoRepository,
    ICurrentUser currentUser)
    : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IOrcamentoRepository _orcamentoRepository = orcamentoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly ICurrentUser _currentUser = currentUser;

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

    public async Task<AcompanhamentoOrdemServicoDto?>
    ObterAcompanhamentoAsync(string placa)
{
    placa = placa
        .Trim()
        .Replace("-", "")
        .ToUpperInvariant();

    var ordemServico =
        await _ordemServicoRepository
            .ObterAtualPorPlacaAsync(placa);

    if (ordemServico is null)
        return null;

    var cliente =
        await _clienteRepository.ObterPorIdAsync(
            ordemServico.ClienteId);

    var veiculo =
        await _veiculoRepository.ObterPorIdAsync(
            ordemServico.VeiculoId);

    if (cliente is null || veiculo is null)
        return null;

    return new AcompanhamentoOrdemServicoDto
    {
        Cliente = cliente.Nome,
        Veiculo = $"{veiculo.Marca} {veiculo.Modelo}",
        Placa = veiculo.Placa,
        Status =
            FormatarStatusOrdemServico(
                ordemServico.Status),
        DataInicio = ordemServico.DataInicio,

        Servicos = ordemServico.Servicos
            .Where(x => x.Ativo)
            .Select(x => new AcompanhamentoServicoDto
            {
                Nome = x.NomeServico,
                Status =
                    FormatarStatusServico(x.Status)
            })
            .ToList()
    };
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
                    x.ServicoId.GetValueOrDefault(),
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
                    x.ItemEstoqueId.GetValueOrDefault(),
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
            ordemServico.Servicos
                .FirstOrDefault(x =>
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
            .AtualizarServicoStatusAsync(servico);

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

    public async Task<TempoMedioOrdensServicoDto>
    ObterTempoMedioAsync()
    {
        var tempos =
            (await _ordemServicoRepository
                .ObterTemposOrdensAsync())
            .ToList();

        if (tempos.Count == 0)
        {
            return new TempoMedioOrdensServicoDto
            {
                QuantidadeOrdens = 0,
                TempoMedioGeral = "0min",
                Ordens = []
            };
        }

        var duracoes = tempos
            .Select(x =>
                x.DataFinalizacao - x.DataInicio)
            .ToList();

        var mediaSegundos =
            duracoes.Average(x => x.TotalSeconds);

        return new TempoMedioOrdensServicoDto
        {
            QuantidadeOrdens = tempos.Count,

            TempoMedioGeral =
                FormatarTempo(mediaSegundos),

            Ordens = tempos
                .Select(x =>
                {
                    var duracao =
                        x.DataFinalizacao - x.DataInicio;

                    return new TempoOrdemServicoDto
                    {
                        OrdemServicoId = x.OrdemServicoId,
                        DataInicio = x.DataInicio,
                        DataFinalizacao = x.DataFinalizacao,
                        TempoExecucao =
                            FormatarTempo(duracao.TotalSeconds)
                    };
                })
                .ToList()
        };
    }



    private static string FormatarTempo(
    double segundos)
    {
        var minutosTotais =
            (long)Math.Round(
                segundos / 60,
                MidpointRounding.AwayFromZero);

        var dias =
            minutosTotais / 1440;

        var horas =
            (minutosTotais % 1440) / 60;

        var minutos =
            minutosTotais % 60;

        if (dias > 0)
            return $"{dias}d {horas}h {minutos}min";

        if (horas > 0)
            return $"{horas}h {minutos}min";

        return $"{minutos}min";
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

    private static string FormatarStatusOrdemServico(
    StatusOrdemServico status)
    {
        return status switch
        {
            StatusOrdemServico.AguardandoExecucao =>
                "Aguardando execução",

            StatusOrdemServico.EmExecucao =>
                "Em execução",

            StatusOrdemServico.Finalizada =>
                "Finalizada",

            StatusOrdemServico.Entregue =>
                "Entregue",

            _ => status.ToString()
        };
    }

    private static string FormatarStatusServico(
        StatusServico status)
    {
        return status switch
        {
            StatusServico.AguardandoExecucao =>
                "Aguardando execução",

            StatusServico.EmExecucao =>
                "Em execução",

            StatusServico.Finalizada =>
                "Finalizado",

            _ => status.ToString()
        };
    }
}
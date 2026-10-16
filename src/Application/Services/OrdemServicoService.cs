using Application.DTOs.Orcamento;
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
    IServicoRepository servicoRepository,
    IItemEstoqueRepository itemEstoqueRepository,
    IUnitOfWork unitOfWork,
    ICurrentUser currentUser)
    : IOrdemServicoService
{
    private readonly IOrdemServicoRepository _ordemServicoRepository = ordemServicoRepository;
    private readonly IOrcamentoRepository _orcamentoRepository = orcamentoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly IServicoRepository _servicoRepository = servicoRepository;
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ICurrentUser _currentUser = currentUser;

    private static readonly StatusOrdemServico[] StatusEmAndamento =
    [
        StatusOrdemServico.EmExecucao,
        StatusOrdemServico.AguardandoExecucao
    ];

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

    public async Task<IEnumerable<OrdemServicoDto>> ObterEmAndamentoAsync()
    {
        var ordens =
            await _ordemServicoRepository.ObterTodosAsync();

        // Ordena pela prioridade do status e, dentro de cada status, das mais antigas para as mais recentes
        return ordens
            .Where(x => StatusEmAndamento.Contains(x.Status))
            .OrderBy(x => Array.IndexOf(StatusEmAndamento, x.Status))
            .ThenBy(x => x.DataCriacao)
            .Select(MapearParaDto);
    }

    public async Task<StatusOrdemServicoDto?> ObterStatusAsync(Guid id)
    {
        var ordemServico =
            await _ordemServicoRepository.ObterPorIdAsync(id);

        if (ordemServico is null)
            return null;

        return new StatusOrdemServicoDto
        {
            OrdemServicoId = ordemServico.Id,
            Status = ordemServico.Status,
            Descricao =
                FormatarStatusOrdemServico(
                    ordemServico.Status)
        };
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

    public async Task<AberturaOrdemServicoDto> AbrirAsync(
        AbrirOrdemServicoDto dto)
    {
        ValidarAbertura(dto);

        await ValidarClienteEVeiculoAsync(
            dto.ClienteId,
            dto.VeiculoId);

        // 1. Gera o orçamento vinculado com os itens informados
        var orcamento = new Orcamento(
            dto.ClienteId,
            dto.VeiculoId,
            _currentUser.Id);

        foreach (var servicoDto in dto.Servicos)
        {
            var servico =
                await _servicoRepository.ObterPorIdAsync(
                    servicoDto.ServicoId)
                ?? throw new DomainException(
                    "Serviço não encontrado.");

            if (!servico.Ativo)
            {
                throw new DomainException(
                    "Não é possível abrir uma ordem de serviço com um serviço inativo.");
            }

            orcamento.AdicionarItem(
                new OrcamentoItem(
                    orcamento.Id,
                    servico.Id,
                    null,
                    servico.Nome,
                    servico.Descricao,
                    servicoDto.Quantidade,
                    servicoDto.ValorUnitario ?? servico.Preco,
                    _currentUser.Id),
                _currentUser.Id);
        }

        foreach (var itemDto in dto.Itens)
        {
            var itemEstoque =
                await _itemEstoqueRepository.ObterPorIdAsync(
                    itemDto.ItemEstoqueId)
                ?? throw new DomainException(
                    "Item de estoque não encontrado.");

            if (!itemEstoque.Ativo)
            {
                throw new DomainException(
                    "Não é possível abrir uma ordem de serviço com um item de estoque inativo.");
            }

            orcamento.AdicionarItem(
                new OrcamentoItem(
                    orcamento.Id,
                    null,
                    itemEstoque.Id,
                    itemEstoque.Nome,
                    itemEstoque.Descricao,
                    itemDto.Quantidade,
                    itemDto.ValorUnitario ?? itemEstoque.Preco,
                    _currentUser.Id),
                _currentUser.Id);
        }

        if (dto.Desconto != 0)
        {
            orcamento.AplicarDesconto(
                dto.Desconto,
                _currentUser.Id);
        }

        // 2. Aprova o orçamento, pois a abertura já representa a autorização do atendimento
        orcamento.ColocarEmAguardandoCliente(
            _currentUser.Id);

        orcamento.Aprovar(
            _currentUser.Id);

        // 3. Grava o orçamento e gera a Ordem de Serviço na mesma transação
        OrdemServicoDto ordemServico = null!;

        await _unitOfWork.ExecutarEmTransacaoAsync(async () =>
        {
            await _orcamentoRepository
                .AdicionarAsync(orcamento);

            await _orcamentoRepository
                .AtualizarItensAsync(orcamento);

            await _orcamentoRepository
                .AtualizarAsync(orcamento);

            ordemServico =
                await CriarAsync(orcamento.Id);
        });

        // 4. Somente depois verifica o estoque
        var itensInsuficientes =
            await VerificarEstoqueInsuficienteAsync(
                orcamento);

        return new AberturaOrdemServicoDto
        {
            OrdemServicoId = ordemServico.Id,
            OrcamentoId = orcamento.Id,

            Mensagem = itensInsuficientes.Count == 0
                ? "Ordem de serviço aberta."
                : "Ordem de serviço aberta, mas existem itens com estoque insuficiente.",

            ItensInsuficientes = itensInsuficientes
        };
    }

    private static void ValidarAbertura(
        AbrirOrdemServicoDto dto)
    {
        if (dto.Servicos.Count == 0)
        {
            throw new DomainException(
                "A ordem de serviço deve possuir pelo menos um serviço.");
        }

        if (dto.Servicos
                .GroupBy(x => x.ServicoId)
                .Any(x => x.Count() > 1))
        {
            throw new DomainException(
                "O mesmo serviço não pode ser informado mais de uma vez.");
        }

        if (dto.Itens
                .GroupBy(x => x.ItemEstoqueId)
                .Any(x => x.Count() > 1))
        {
            throw new DomainException(
                "O mesmo item de estoque não pode ser informado mais de uma vez.");
        }
    }

    private async Task ValidarClienteEVeiculoAsync(
        Guid clienteId,
        Guid veiculoId)
    {
        var cliente =
            await _clienteRepository.ObterPorIdAsync(clienteId)
            ?? throw new DomainException(
                "Cliente não encontrado.");

        if (!cliente.Ativo)
        {
            throw new DomainException(
                "Não é possível abrir uma ordem de serviço para um cliente inativo.");
        }

        var veiculo =
            await _veiculoRepository.ObterPorIdAsync(veiculoId)
            ?? throw new DomainException(
                "Veículo não encontrado.");

        if (!veiculo.Ativo)
        {
            throw new DomainException(
                "Não é possível abrir uma ordem de serviço para um veículo inativo.");
        }

        if (veiculo.ClienteId != clienteId)
        {
            throw new DomainException(
                "O veículo informado não pertence ao cliente.");
        }
    }

    private async Task<List<ItemEstoqueInsuficienteDto>>
    VerificarEstoqueInsuficienteAsync(Orcamento orcamento)
    {
        var resultado = new List<ItemEstoqueInsuficienteDto>();

        foreach (var item in orcamento.Itens
                     .Where(x => x.Ativo && x.ItemEstoqueId.HasValue))
        {
            var itemEstoque =
                await _itemEstoqueRepository.ObterPorIdAsync(
                    item.ItemEstoqueId.GetValueOrDefault());

            if (itemEstoque is null)
                continue;

            if (itemEstoque.Estoque >= item.Quantidade)
                continue;

            resultado.Add(new ItemEstoqueInsuficienteDto
            {
                ItemEstoqueId = itemEstoque.Id,
                Nome = itemEstoque.Nome,
                QuantidadeDisponivel = itemEstoque.Estoque,
                QuantidadeNecessaria = item.Quantidade,
                QuantidadeFaltante =
                    item.Quantidade - itemEstoque.Estoque
            });
        }

        return resultado;
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
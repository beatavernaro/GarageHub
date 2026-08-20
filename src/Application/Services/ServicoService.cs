using Application.DTOs.Servico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Enums;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class ServicoService(IServicoRepository servicoRepository, IItemEstoqueRepository itemEstoqueRepository, ICurrentUser currentUser) : IServicoService
{
    private readonly IServicoRepository _servicoRepository = servicoRepository;

    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;

    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ServicoDto?> ObterPorIdAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id);

        return servico is null
            ? null
            : MapearParaDto(servico);
    }

    public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
    {
        var servicos = await _servicoRepository.ObterTodosAsync();

        return servicos.Select(MapearParaDto);
    }

    public async Task<ServicoDto?> ObterPorNomeAsync(string nome)
    {
        var servico = await _servicoRepository.ObterPorNomeAsync(nome);

        return servico is null
            ? null
            : MapearParaDto(servico);
    }

    public async Task<ServicoDto> CriarAsync(
        CriarServicoDto dto)
    {
        var nome = dto.Nome.Trim();

        var servicoExistente = await _servicoRepository.ObterPorNomeAsync(nome);

        if (servicoExistente is not null)
            throw new DomainException("Já existe um serviço cadastrado com este nome.");

        var servico = new Servico(
            nome,
            dto.Descricao,
            dto.Preco,
            dto.Status,
            _currentUser.Id);

        servico.Normalizar();

        await _servicoRepository.AdicionarAsync(servico);

        return MapearParaDto(servico);
    }

    public async Task AtualizarAsync(Guid id, AtualizarServicoDto dto)
    {
        var servico =
            await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        var nome = dto.Nome.Trim();

        var outroServico = await _servicoRepository.ObterPorNomeAsync(nome);

        if (outroServico is not null &&
            outroServico.Id != id)
        {
            throw new DomainException("Já existe outro serviço cadastrado com este nome.");
        }

        servico.Atualizar(
            nome,
            dto.Descricao,
            _currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AlterarPrecoAsync(Guid id, decimal novoPreco)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.AlterarPreco(novoPreco, _currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AlterarStatusAsync(Guid id, StatusServico status)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.AlterarStatus(status, _currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AdicionarItemEstoqueAsync(Guid id, AdicionarServicoItemEstoqueDto dto)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        var item = await _itemEstoqueRepository.ObterPorIdAsync(
                dto.ItemEstoqueId)
            ?? throw new DomainException("Item de estoque não encontrado.");

        servico.AdicionarPecaInsumo(
            item,
            dto.Quantidade,
            _currentUser.Id);

        var itemServico = servico.ItensEstoque
                .First(x =>
                    x.ItemEstoqueId == dto.ItemEstoqueId &&
                    x.Ativo);

        await _servicoRepository.AdicionarItemEstoqueAsync(itemServico);
    }

    public async Task AlterarQuantidadeItemEstoqueAsync(Guid id, Guid itemEstoqueId, int quantidade)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.AlterarQuantidadeItemEstoque(
            itemEstoqueId,
            quantidade,
            _currentUser.Id);

        var item = servico.ItensEstoque
                .First(x =>
                    x.ItemEstoqueId == itemEstoqueId &&
                    x.Ativo);

        await _servicoRepository.AtualizarItemEstoqueAsync(item);
    }

    public async Task RemoverItemEstoqueAsync(Guid id, Guid itemEstoqueId)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.RemoverItemEstoque(
            itemEstoqueId,
            _currentUser.Id);

        var item = servico.ItensEstoque
                .FirstOrDefault(x =>
                    x.ItemEstoqueId == itemEstoqueId) ?? throw new DomainException("Item de estoque não encontrado no serviço.");

        await _servicoRepository.InativarItemEstoqueAsync(
            item.Id,
            item.DataAlteracao!.Value,
            item.AlteradoPorId!.Value);
    }

    public async Task InativarAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.Desativar(_currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AtivarAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.Ativar(_currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    private static ServicoDto MapearParaDto(Servico servico)
    {
        return new ServicoDto
        {
            Id = servico.Id,
            Nome = servico.Nome,
            Descricao = servico.Descricao,
            Preco = servico.Preco,
            Status = servico.Status,
            Ativo = servico.Ativo,

            ItensEstoque = [.. servico.ItensEstoque
                .Where(x => x.Ativo)
                .Select(x => new ServicoItemEstoqueDto
                {
                    Id = x.Id,
                    ItemEstoqueId = x.ItemEstoqueId,
                    Quantidade = x.Quantidade,
                    Ativo = x.Ativo
                })]
        };
    }
}
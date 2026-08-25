using Application.DTOs.ItemEstoque;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class ItemEstoqueService(
    IItemEstoqueRepository itemEstoqueRepository,
    ICurrentUser currentUser) : IItemEstoqueService
{
    private readonly IItemEstoqueRepository _itemEstoqueRepository = itemEstoqueRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private const string ItemNaoEncontrado = "Item de estoque não encontrado.";

    public async Task<ItemEstoqueDto?> ObterPorIdAsync(Guid id)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id);

        return item is null
            ? null
            : MapearParaDto(item);
    }

    public async Task<IEnumerable<ItemEstoqueDto>> ObterTodosAsync()
    {
        var itens = await _itemEstoqueRepository.ObterTodosAsync();

        return itens.Select(MapearParaDto);
    }

    public async Task<ItemEstoqueDto?> ObterPorCodigoInternoAsync(string codigoInterno)
    {
        var item = await _itemEstoqueRepository.ObterPorCodigoInternoAsync(codigoInterno);

        return item is null
            ? null
            : MapearParaDto(item);
    }

    public async Task<ItemEstoqueDto> CriarAsync(CriarItemEstoqueDto dto)
    {
        var codigoInterno = NormalizarCodigo(dto.CodigoInterno);

        var itemExistente =
            await _itemEstoqueRepository.ObterPorCodigoInternoAsync(codigoInterno);

        if (itemExistente is not null)
            throw new DomainException("Já existe um item cadastrado com este código interno.");

        var item = new ItemEstoque(
            codigoInterno,
            dto.Nome,
            dto.Tipo,
            dto.Preco,
            dto.Estoque,
            _currentUser.Id,
            dto.Descricao);

        await _itemEstoqueRepository.AdicionarAsync(item);

        return MapearParaDto(item);
    }

    public async Task AtualizarAsync(Guid id, AtualizarItemEstoqueDto dto)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(ItemNaoEncontrado);

        var codigoInterno = NormalizarCodigo(dto.CodigoInterno);

        var outroItem = await _itemEstoqueRepository.ObterPorCodigoInternoAsync(codigoInterno);

        if (outroItem is not null && outroItem.Id != id)
            throw new DomainException("Já existe outro item cadastrado com este código interno.");

        item.Atualizar(
    codigoInterno,
    dto.Nome,
    dto.Descricao,
    dto.Tipo,
    _currentUser.Id);

        item.AlterarPreco(
    dto.Preco,
    _currentUser.Id);

        await _itemEstoqueRepository.AtualizarAsync(item);
    }

    public async Task AdicionarEstoqueAsync(Guid id, int quantidade)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(ItemNaoEncontrado);

        item.AdicionarEstoque(quantidade, _currentUser.Id);

        await _itemEstoqueRepository.AtualizarAsync(item);
    }

    public async Task RemoverEstoqueAsync(Guid id, int quantidade)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(ItemNaoEncontrado);

        item.RemoverEstoque(quantidade, _currentUser.Id);

        await _itemEstoqueRepository.AtualizarAsync(item);
    }

    public async Task AlterarPrecoAsync(Guid id, decimal novoPreco)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(ItemNaoEncontrado);

        item.AlterarPreco(novoPreco, _currentUser.Id);

        await _itemEstoqueRepository.AtualizarAsync(item);
    }

    public async Task InativarAsync(Guid id)
    {
        var item = await _itemEstoqueRepository.ObterPorIdAsync(id)
            ?? throw new DomainException(ItemNaoEncontrado);

        item.Desativar(_currentUser.Id);

        await _itemEstoqueRepository.AtualizarAsync(item);
    }

    private static ItemEstoqueDto MapearParaDto(ItemEstoque item)
    {
        return new ItemEstoqueDto
        {
            Id = item.Id,
            CodigoInterno = item.CodigoInterno,
            Nome = item.Nome,
            Descricao = item.Descricao,
            Tipo = item.Tipo,
            Preco = item.Preco,
            Estoque = item.Estoque,
            Ativo = item.Ativo
        };
    }

    private static string NormalizarCodigo(string codigo)
    {
        return codigo.Trim().ToUpperInvariant();
    }
}
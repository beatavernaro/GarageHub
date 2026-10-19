using Application.DTOs.ItemEstoque;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class ItensEstoqueController(IItemEstoqueService itemEstoqueService) : ControllerBase
{
    private readonly IItemEstoqueService _itemEstoqueService = itemEstoqueService;

    [HttpGet]
    [EndpointSummary("Lista os itens de estoque")]
    [EndpointDescription("Retorna todas as peças e insumos ativos cadastrados.")]
    [ProducesResponseType(typeof(IEnumerable<ItemEstoqueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ItemEstoqueDto>>> ObterTodos()
    {
        var itens = await _itemEstoqueService.ObterTodosAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um item de estoque pelo ID")]
    [EndpointDescription("Retorna os dados de uma peça ou insumo cadastrado.")]
    [ProducesResponseType(typeof(ItemEstoqueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemEstoqueDto>> ObterPorId(Guid id)
    {
        var item = await _itemEstoqueService.ObterPorIdAsync(id);

        return item is null
            ? NotFound()
            : Ok(item);
    }

    [HttpGet("codigo/{codigoInterno}")]
    [EndpointSummary("Obtém um item de estoque pelo código interno")]
    [EndpointDescription("Retorna os dados da peça ou insumo cadastrado com o código interno informado.")]
    [ProducesResponseType(typeof(ItemEstoqueDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ItemEstoqueDto>> ObterPorCodigoInterno(string codigoInterno)
    {
        var item = await _itemEstoqueService.ObterPorCodigoInternoAsync(codigoInterno);

        return item is null
            ? NotFound()
            : Ok(item);
    }

    [HttpPost]
    [EndpointSummary("Cadastra um item de estoque")]
    [EndpointDescription("Cadastra uma peça ou insumo com preço e quantidade inicial em estoque.")]
    [ProducesResponseType(typeof(ItemEstoqueDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ItemEstoqueDto>> Criar([FromBody] CriarItemEstoqueDto dto)
    {
        var item = await _itemEstoqueService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = item.Id },
            item);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Atualiza um item de estoque")]
    [EndpointDescription("Atualiza os dados cadastrais de uma peça ou insumo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarItemEstoqueDto dto)
    {
        await _itemEstoqueService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/adicionar-estoque")]
    [EndpointSummary("Adiciona quantidade ao estoque")]
    [EndpointDescription("Soma a quantidade informada ao estoque atual do item.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarEstoque(Guid id, [FromBody] int quantidade)
    {
        await _itemEstoqueService.AdicionarEstoqueAsync(id, quantidade);

        return NoContent();
    }

    [HttpPatch("{id:guid}/remover-estoque")]
    [EndpointSummary("Remove quantidade do estoque")]
    [EndpointDescription("Subtrai a quantidade informada do estoque atual do item. O estoque não pode ficar negativo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverEstoque(Guid id, [FromBody] int quantidade)
    {
        await _itemEstoqueService.RemoverEstoqueAsync(id, quantidade);

        return NoContent();
    }

    [HttpPatch("{id:guid}/preco")]
    [EndpointSummary("Altera o preço de um item de estoque")]
    [EndpointDescription("Altera o preço unitário da peça ou insumo. O novo preço deve ser maior que zero.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarPreco(Guid id, [FromBody] decimal novoPreco)
    {
        await _itemEstoqueService.AlterarPrecoAsync(id, novoPreco);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [EndpointSummary("Inativa um item de estoque")]
    [EndpointDescription("Inativa a peça ou insumo, que deixa de ser listado entre os itens ativos.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(Guid id)
    {
        await _itemEstoqueService.InativarAsync(id);

        return NoContent();
    }
}
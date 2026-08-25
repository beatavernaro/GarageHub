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
    [ProducesResponseType(typeof(IEnumerable<ItemEstoqueDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ItemEstoqueDto>>> ObterTodos()
    {
        var itens = await _itemEstoqueService.ObterTodosAsync();

        return Ok(itens);
    }

    [HttpGet("{id:guid}")]
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
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarItemEstoqueDto dto)
    {
        await _itemEstoqueService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/adicionar-estoque")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarEstoque(Guid id, [FromBody] int quantidade)
    {
        await _itemEstoqueService.AdicionarEstoqueAsync(id, quantidade);

        return NoContent();
    }

    [HttpPatch("{id:guid}/remover-estoque")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverEstoque(Guid id, [FromBody] int quantidade)
    {
        await _itemEstoqueService.RemoverEstoqueAsync(id, quantidade);

        return NoContent();
    }

    [HttpPatch("{id:guid}/preco")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarPreco(Guid id, [FromBody] decimal novoPreco)
    {
        await _itemEstoqueService.AlterarPrecoAsync(id, novoPreco);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(Guid id)
    {
        await _itemEstoqueService.InativarAsync(id);

        return NoContent();
    }
}
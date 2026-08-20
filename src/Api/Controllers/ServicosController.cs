using Application.DTOs.Servico;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicosController(
    IServicoService servicoService) : ControllerBase
{
    private readonly IServicoService _servicoService =
        servicoService;

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServicoDto>>> ObterTodos()
    {
        var servicos =
            await _servicoService.ObterTodosAsync();

        return Ok(servicos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ServicoDto>> ObterPorId(
        Guid id)
    {
        var servico =
            await _servicoService.ObterPorIdAsync(id);

        if (servico is null)
            return NotFound();

        return Ok(servico);
    }

    [HttpGet("nome/{nome}")]
    public async Task<ActionResult<ServicoDto>> ObterPorNome(
        string nome)
    {
        var servico =
            await _servicoService.ObterPorNomeAsync(nome);

        if (servico is null)
            return NotFound();

        return Ok(servico);
    }

    [HttpPost]
    public async Task<ActionResult<ServicoDto>> Criar(
        CriarServicoDto dto)
    {
        var servico =
            await _servicoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = servico.Id },
            servico);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Atualizar(
        Guid id,
        AtualizarServicoDto dto)
    {
        await _servicoService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/preco")]
    public async Task<IActionResult> AlterarPreco(
        Guid id,
        [FromQuery] decimal novoPreco)
    {
        await _servicoService.AlterarPrecoAsync(
            id,
            novoPreco);

        return NoContent();
    }

    [HttpPost("{id:guid}/itens-estoque")]
    public async Task<IActionResult> AdicionarItemEstoque(
        Guid id,
        AdicionarServicoItemEstoqueDto dto)
    {
        await _servicoService.AdicionarItemEstoqueAsync(
            id,
            dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/itens-estoque/{itemEstoqueId:guid}")]
    public async Task<IActionResult> AlterarQuantidadeItemEstoque(
        Guid id,
        Guid itemEstoqueId,
        [FromQuery] int quantidade)
    {
        await _servicoService.AlterarQuantidadeItemEstoqueAsync(
            id,
            itemEstoqueId,
            quantidade);

        return NoContent();
    }

    [HttpDelete("{id:guid}/itens-estoque/{itemEstoqueId:guid}")]
    public async Task<IActionResult> RemoverItemEstoque(
        Guid id,
        Guid itemEstoqueId)
    {
        await _servicoService.RemoverItemEstoqueAsync(
            id,
            itemEstoqueId);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    public async Task<IActionResult> Inativar(Guid id)
    {
        await _servicoService.InativarAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    public async Task<IActionResult> Ativar(Guid id)
    {
        await _servicoService.AtivarAsync(id);

        return NoContent();
    }
}
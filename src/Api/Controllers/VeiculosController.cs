using Application.DTOs.Veiculo;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
[Produces("application/json")]
public class VeiculosController(IVeiculoService veiculoService) : ControllerBase
{
    private readonly IVeiculoService _veiculoService = veiculoService;

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<VeiculoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> ObterTodos()
    {
        var veiculos = await _veiculoService.ObterTodosAsync();

        return Ok(veiculos);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(VeiculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VeiculoDto>> ObterPorId(Guid id)
    {
        var veiculo = await _veiculoService.ObterPorIdAsync(id);

        if (veiculo is null)
            return NotFound();

        return Ok(veiculo);
    }

    [HttpGet("placa/{placa}")]
    [ProducesResponseType(typeof(VeiculoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VeiculoDto>> ObterPorPlaca(string placa)
    {
        var veiculo = await _veiculoService.ObterPorPlacaAsync(placa);

        if (veiculo is null)
            return NotFound();

        return Ok(veiculo);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> ObterPorClienteId(Guid clienteId)
    {
        var veiculos = await _veiculoService.ObterPorClienteIdAsync(clienteId);

        return Ok(veiculos);
    }

    [HttpPost]
    [ProducesResponseType(typeof(VeiculoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<VeiculoDto>> Criar([FromBody] CriarVeiculoDto dto)
    {
        var veiculo = await _veiculoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = veiculo.Id },
            veiculo);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarVeiculoDto dto)
    {
        await _veiculoService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(Guid id)
    {
        await _veiculoService.InativarAsync(id);

        return NoContent();
    }
}

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
    [EndpointSummary("Lista os veículos")]
    [EndpointDescription("Retorna todos os veículos ativos cadastrados.")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> ObterTodos()
    {
        var veiculos = await _veiculoService.ObterTodosAsync();

        return Ok(veiculos);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um veículo pelo ID")]
    [EndpointDescription("Retorna os dados de um veículo cadastrado.")]
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
    [EndpointSummary("Obtém um veículo pela placa")]
    [EndpointDescription("Retorna os dados do veículo cadastrado com a placa informada.")]
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
    [EndpointSummary("Lista os veículos de um cliente")]
    [EndpointDescription("Retorna os veículos vinculados ao cliente informado.")]
    [ProducesResponseType(typeof(IEnumerable<VeiculoDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<VeiculoDto>>> ObterPorClienteId(Guid clienteId)
    {
        var veiculos = await _veiculoService.ObterPorClienteIdAsync(clienteId);

        return Ok(veiculos);
    }

    [HttpPost]
    [EndpointSummary("Cadastra um veículo")]
    [EndpointDescription("Cadastra um veículo vinculado a um cliente.")]
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
    [EndpointSummary("Atualiza um veículo")]
    [EndpointDescription("Atualiza os dados do veículo, incluindo o cliente ao qual está vinculado.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar(Guid id, [FromBody] AtualizarVeiculoDto dto)
    {
        await _veiculoService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [EndpointSummary("Inativa um veículo")]
    [EndpointDescription("Inativa o veículo, que deixa de ser listado entre os veículos ativos.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar(Guid id)
    {
        await _veiculoService.InativarAsync(id);

        return NoContent();
    }
}

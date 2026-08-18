using Application.DTOs.Cliente;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/clientes")]
[Produces("application/json")]
[Tags("Clientes")]
public class ClientesController(IClienteService clienteService) : ControllerBase
{
    private readonly IClienteService _clienteService = clienteService;

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um cliente pelo ID")]
    [EndpointDescription("Retorna os dados de um cliente cadastrado.")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> ObterPorId([FromRoute] Guid id)
    {
        var cliente = await _clienteService.ObterPorIdAsync(id);

        return Ok(cliente);
    }

    [HttpGet]
    [EndpointSummary("Lista os clientes")]
    [EndpointDescription("Retorna todos os clientes ativos.")]
    [ProducesResponseType(typeof(IEnumerable<ClienteDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ClienteDto>>> ObterTodos()
    {
        var clientes = await _clienteService.ObterTodosAsync();

        return Ok(clientes);
    }

    [HttpGet("documento/{documento}")]
    [EndpointSummary("Obtém um cliente pelo documento")]
    [EndpointDescription("Retorna um cliente através do CPF ou CNPJ.")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> ObterPorDocumento([FromRoute] string documento)
    {
        var cliente = await _clienteService.ObterPorDocumentoAsync(documento);

        return Ok(cliente);
    }

    [HttpPost]
    [Consumes("application/json")]
    [EndpointSummary("Cadastra um cliente")]
    [EndpointDescription("Cria um novo cliente.")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ClienteDto>> Criar([FromBody] CriarClienteDto dto)
    {
        var cliente = await _clienteService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = cliente.Id },
            cliente);
    }

    [HttpPut("{id:guid}")]
    [Consumes("application/json")]
    [EndpointSummary("Atualiza um cliente")]
    [EndpointDescription("Atualiza os dados de um cliente existente.")]
    [ProducesResponseType(typeof(ClienteDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ClienteDto>> Atualizar([FromRoute] Guid id, [FromBody] AtualizarClienteDto dto)
    {
        await _clienteService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [EndpointSummary("Inativa um cliente")]
    [EndpointDescription("Inativa um cliente existente.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar([FromRoute] Guid id)
    {
        await _clienteService.InativarAsync(id);

        return NoContent();
    }
}


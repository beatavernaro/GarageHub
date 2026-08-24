using Application.DTOs.OrdemServico;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdensServicoController(
    IOrdemServicoService ordemServicoService)
    : ControllerBase
{
    private readonly IOrdemServicoService _ordemServicoService =
        ordemServicoService;

    [HttpGet]
    [EndpointSummary("Obtém todas as ordens de serviço")]
    [EndpointDescription(
        "Retorna todas as ordens de serviço cadastradas.")]
    [ProducesResponseType(
        typeof(IEnumerable<OrdemServicoDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrdemServicoDto>>>
        ObterTodos()
    {
        var ordens =
            await _ordemServicoService.ObterTodosAsync();

        return Ok(ordens);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém uma ordem de serviço pelo ID")]
    [EndpointDescription(
        "Retorna a ordem de serviço com seus serviços e itens de estoque.")]
    [ProducesResponseType(
        typeof(OrdemServicoDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrdemServicoDto>>
        ObterPorId([FromRoute] Guid id)
    {
        var ordemServico =
            await _ordemServicoService.ObterPorIdAsync(id);

        if (ordemServico is null)
            return NotFound();

        return Ok(ordemServico);
    }

    [HttpPatch(
        "{id:guid}/servicos/{ordemServicoServicoId:guid}/status")]
    [EndpointSummary(
        "Altera o status de um serviço da ordem de serviço")]
    [EndpointDescription(
        "Altera o status do serviço e recalcula automaticamente o status da ordem de serviço.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatusServico(
        [FromRoute] Guid id,
        [FromRoute] Guid ordemServicoServicoId,
        [FromBody] StatusServico status)
    {
        await _ordemServicoService
            .AlterarStatusServicoAsync(
                id,
                ordemServicoServicoId,
                status);

        return NoContent();
    }

    [HttpPost("{id:guid}/entregar")]
    [EndpointSummary("Entrega uma ordem de serviço")]
    [EndpointDescription(
        "Marca uma ordem de serviço finalizada como entregue.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Entregar(
        [FromRoute] Guid id)
    {
        await _ordemServicoService
            .EntregarAsync(id);

        return NoContent();
    }
}
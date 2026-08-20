using Application.DTOs.Orcamento;
using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrcamentosController(
    IOrcamentoService orcamentoService) : ControllerBase
{
    private readonly IOrcamentoService _orcamentoService =
        orcamentoService;

    [HttpGet]
    [EndpointSummary("Obtém todos os orçamentos")]
    [EndpointDescription(
        "Retorna todos os orçamentos cadastrados.")]
    [ProducesResponseType(
        typeof(IEnumerable<OrcamentoDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrcamentoDto>>> ObterTodos()
    {
        var orcamentos =
            await _orcamentoService.ObterTodosAsync();

        return Ok(orcamentos);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um orçamento pelo ID")]
    [EndpointDescription(
        "Retorna os dados completos de um orçamento, incluindo seus itens.")]
    [ProducesResponseType(
        typeof(OrcamentoDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrcamentoDto>> ObterPorId(
        [FromRoute] Guid id)
    {
        var orcamento =
            await _orcamentoService.ObterPorIdAsync(id);

        if (orcamento is null)
            return NotFound();

        return Ok(orcamento);
    }

    [HttpGet("cliente/{clienteId:guid}")]
    [EndpointSummary("Obtém os orçamentos de um cliente")]
    [EndpointDescription(
        "Retorna todos os orçamentos vinculados ao cliente informado.")]
    [ProducesResponseType(
        typeof(IEnumerable<OrcamentoDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<OrcamentoDto>>> ObterPorCliente(
        [FromRoute] Guid clienteId)
    {
        var orcamentos =
            await _orcamentoService
                .ObterPorClienteIdAsync(clienteId);

        return Ok(orcamentos);
    }

    [HttpPost]
    [EndpointSummary("Cria um novo orçamento")]
    [EndpointDescription(
        "Cria um orçamento para o cliente e veículo informados.")]
    [ProducesResponseType(
        typeof(OrcamentoDto),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<OrcamentoDto>> Criar(
        [FromBody] CriarOrcamentoDto dto)
    {
        var orcamento =
            await _orcamentoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = orcamento.Id },
            orcamento);
    }

    [HttpPost("{id:guid}/itens")]
    [EndpointSummary("Adiciona um item ao orçamento")]
    [EndpointDescription(
        "Adiciona um serviço ou item de estoque ao orçamento.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AdicionarItem(
        [FromRoute] Guid id,
        [FromBody] AdicionarOrcamentoItemDto dto)
    {
        await _orcamentoService
            .AdicionarItemAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/itens/{itemId:guid}/quantidade")]
    [EndpointSummary("Altera a quantidade de um item")]
    [EndpointDescription(
        "Atualiza a quantidade de um item do orçamento.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarQuantidadeItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        [FromBody] int quantidade)
    {
        await _orcamentoService
            .AlterarQuantidadeItemAsync(
                id,
                itemId,
                quantidade);

        return NoContent();
    }

    [HttpPatch("{id:guid}/itens/{itemId:guid}/valor")]
    [EndpointSummary("Altera o valor de um item")]
    [EndpointDescription(
        "Atualiza o valor unitário de um item do orçamento.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarValorItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId,
        [FromBody] decimal valorUnitario)
    {
        await _orcamentoService
            .AlterarValorUnitarioItemAsync(
                id,
                itemId,
                valorUnitario);

        return NoContent();
    }

    [HttpDelete("{id:guid}/itens/{itemId:guid}")]
    [EndpointSummary("Remove um item do orçamento")]
    [EndpointDescription(
        "Inativa o item dentro do orçamento.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoverItem(
        [FromRoute] Guid id,
        [FromRoute] Guid itemId)
    {
        await _orcamentoService
            .RemoverItemAsync(id, itemId);

        return NoContent();
    }

    [HttpPatch("{id:guid}/desconto")]
    [EndpointSummary("Aplica um desconto ao orçamento")]
    [EndpointDescription(
        "Atualiza o desconto aplicado ao valor total do orçamento.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AplicarDesconto(
        [FromRoute] Guid id,
        [FromBody] decimal desconto)
    {
        await _orcamentoService
            .AplicarDescontoAsync(id, desconto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    [EndpointSummary("Altera o status de um orçamento")]
    [EndpointDescription(
        "Permite enviar o orçamento para aguardando cliente ou cancelá-lo.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarStatus(
        [FromRoute] Guid id,
        [FromBody] StatusOrcamento status)
    {
        await _orcamentoService
            .AlterarStatusAsync(id, status);

        return NoContent();
    }

    [HttpPost("{id:guid}/aprovar")]
    [EndpointSummary("Aprova um orçamento")]
    [EndpointDescription(
        "Aprova o orçamento e verifica se existe estoque suficiente. A aprovação não é impedida por falta de estoque.")]
    [ProducesResponseType(
        typeof(ResultadoAprovacaoOrcamentoDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ResultadoAprovacaoOrcamentoDto>> Aprovar(
        [FromRoute] Guid id)
    {
        var resultado =
            await _orcamentoService.AprovarAsync(id);

        return Ok(resultado);
    }

    [HttpPost("{id:guid}/rejeitar")]
    [EndpointSummary("Rejeita um orçamento")]
    [EndpointDescription(
        "Altera o status do orçamento para rejeitado.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rejeitar(
        [FromRoute] Guid id)
    {
        await _orcamentoService.RejeitarAsync(id);

        return NoContent();
    }

    [HttpPost("{id:guid}/aguardando-cliente")]
    [EndpointSummary("Envia o orçamento para o cliente")]
    [EndpointDescription(
        "Altera o orçamento de elaboração para aguardando cliente e inicia o prazo de 15 dias para expiração.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AguardarCliente(
        [FromRoute] Guid id)
    {
        await _orcamentoService
            .ColocarEmAguardandoClienteAsync(id);

        return NoContent();
    }

    [HttpPost("{id:guid}/cancelar")]
    [EndpointSummary("Cancela um orçamento")]
    [EndpointDescription(
        "Cancela um orçamento que ainda não foi finalizado.")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
        StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Cancelar(
        [FromRoute] Guid id)
    {
        await _orcamentoService.CancelarAsync(id);

        return NoContent();
    }
}
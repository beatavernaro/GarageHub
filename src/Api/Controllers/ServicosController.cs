using Application.DTOs.Servico;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ServicosController(
    IServicoService servicoService) : ControllerBase
{
    private readonly IServicoService _servicoService = servicoService;

    [HttpGet]
    [EndpointSummary("Obtém todos os serviços")]
    [EndpointDescription("Retorna todos os serviços ativos cadastrados.")]
    [ProducesResponseType(typeof(IEnumerable<ServicoDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ServicoDto>>> ObterTodos()
    {
        var servicos =
            await _servicoService.ObterTodosAsync();

        return Ok(servicos);
    }

    [HttpGet("{id:guid}")]
    [EndpointSummary("Obtém um serviço pelo ID")]
    [EndpointDescription("Retorna os dados de um serviço cadastrado.")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServicoDto>> ObterPorId([FromRoute] Guid id)
    {
        var servico = await _servicoService.ObterPorIdAsync(id);

        if (servico is null)
            return NotFound();

        return Ok(servico);
    }

    [HttpGet("nome/{nome}")]
    [EndpointSummary("Obtém um serviço pelo nome")]
    [EndpointDescription(
        "Retorna os dados de um serviço cadastrado com o nome informado.")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ServicoDto>> ObterPorNome([FromRoute] string nome)
    {
        var servico = await _servicoService.ObterPorNomeAsync(nome);

        if (servico is null)
            return NotFound();

        return Ok(servico);
    }

    [HttpPost]
    [EndpointSummary("Cria um novo serviço")]
    [EndpointDescription("Cadastra um novo serviço.")]
    [ProducesResponseType(typeof(ServicoDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ServicoDto>> Criar([FromBody] CriarServicoDto dto)
    {
        var servico = await _servicoService.CriarAsync(dto);

        return CreatedAtAction(
            nameof(ObterPorId),
            new { id = servico.Id },
            servico);
    }

    [HttpPut("{id:guid}")]
    [EndpointSummary("Atualiza um serviço")]
    [EndpointDescription("Atualiza os dados cadastrais de um serviço.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Atualizar([FromRoute] Guid id, [FromBody] AtualizarServicoDto dto)
    {
        await _servicoService.AtualizarAsync(id, dto);

        return NoContent();
    }

    [HttpPatch("{id:guid}/preco")]
    [EndpointSummary("Altera o preço de um serviço")]
    [EndpointDescription("Atualiza apenas o preço do serviço informado.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AlterarPreco([FromRoute] Guid id, [FromQuery] decimal novoPreco)
    {
        await _servicoService.AlterarPrecoAsync(id, novoPreco);

        return NoContent();
    }

    [HttpPatch("{id:guid}/inativar")]
    [EndpointSummary("Inativa um serviço")]
    [EndpointDescription("Altera o serviço para inativo sem removê-lo do banco de dados.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Inativar([FromRoute] Guid id)
    {
        await _servicoService.InativarAsync(id);

        return NoContent();
    }

    [HttpPatch("{id:guid}/ativar")]
    [EndpointSummary("Ativa um serviço")]
    [EndpointDescription("Altera o serviço para ativo.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Ativar([FromRoute] Guid id)
    {
        await _servicoService.AtivarAsync(id);

        return NoContent();
    }
}
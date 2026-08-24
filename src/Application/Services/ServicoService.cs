using Application.DTOs.Servico;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class ServicoService(IServicoRepository servicoRepository, ICurrentUser currentUser) : IServicoService
{
    private readonly IServicoRepository _servicoRepository = servicoRepository;

    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<ServicoDto?> ObterPorIdAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id);

        return servico is null
            ? null
            : MapearParaDto(servico);
    }

    public async Task<IEnumerable<ServicoDto>> ObterTodosAsync()
    {
        var servicos = await _servicoRepository.ObterTodosAsync();

        return servicos.Select(MapearParaDto);
    }

    public async Task<ServicoDto?> ObterPorCodigoInternoAsync(string codigoInterno)
    {
        var servico = await _servicoRepository.ObterPorCodigoInternoAsync(codigoInterno);

        return servico is null
            ? null
            : MapearParaDto(servico);
    }

    public async Task<ServicoDto> CriarAsync(
        CriarServicoDto dto)
    {
        var codigoInterno = dto.CodigoInterno.Trim();

        var servicoExistente = await _servicoRepository.ObterPorCodigoInternoAsync(codigoInterno);

        if (servicoExistente is not null)
            throw new DomainException("Já existe um serviço cadastrado com este código interno.");

        var servico = new Servico(
            codigoInterno,
            dto.Nome,
            dto.Descricao,
            dto.Preco,
            _currentUser.Id);

        servico.Normalizar();

        await _servicoRepository.AdicionarAsync(servico);

        return MapearParaDto(servico);
    }

    public async Task AtualizarAsync(Guid id, AtualizarServicoDto dto)
    {
        var servico =
            await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        var codigoInterno = dto.CodigoInterno.Trim();

        var outroServico = await _servicoRepository.ObterPorCodigoInternoAsync(codigoInterno);

        if (outroServico is not null &&
            outroServico.Id != id)
        {
            throw new DomainException("Já existe outro serviço cadastrado com este código interno.");
        }

        servico.Atualizar(
            codigoInterno,
            dto.Nome,
            dto.Descricao,
            _currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AlterarPrecoAsync(Guid id, decimal novoPreco)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.AlterarPreco(novoPreco, _currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task InativarAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.Desativar(_currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    public async Task AtivarAsync(Guid id)
    {
        var servico = await _servicoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Serviço não encontrado.");

        servico.Ativar(_currentUser.Id);

        await _servicoRepository.AtualizarAsync(servico);
    }

    private static ServicoDto MapearParaDto(Servico servico)
{
    return new ServicoDto
    {
        Id = servico.Id,
        CodigoInterno = servico.CodigoInterno,
        Nome = servico.Nome,
        Descricao = servico.Descricao,
        Preco = servico.Preco,
        Ativo = servico.Ativo
    };
}
}
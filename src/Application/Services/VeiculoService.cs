using Application.DTOs.Veiculo;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Exceptions;

namespace Application.Services;

public class VeiculoService(IVeiculoRepository veiculoRepository, IClienteRepository clienteRepository, ICurrentUser currentUser) : IVeiculoService
{
    private readonly IVeiculoRepository _veiculoRepository = veiculoRepository;
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ICurrentUser _currentUser = currentUser;

    public async Task<VeiculoDto?> ObterPorIdAsync(Guid id)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id);

        return veiculo is null
            ? null
            : MapearParaDto(veiculo);
    }

    public async Task<IEnumerable<VeiculoDto>> ObterTodosAsync()
    {
        var veiculos = await _veiculoRepository.ObterTodosAsync();

        return veiculos.Select(MapearParaDto);
    }

    public async Task<IEnumerable<VeiculoDto>> ObterPorClienteIdAsync(Guid clienteId)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(clienteId) ?? throw new DomainException("Cliente não encontrado.");
        var veiculos = await _veiculoRepository.ObterPorClienteIdAsync(clienteId);

        return veiculos.Select(MapearParaDto);
    }

    public async Task<VeiculoDto?> ObterPorPlacaAsync(string placa)
    {
        var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa);

        return veiculo is null
            ? null
            : MapearParaDto(veiculo);
    }

    public async Task<VeiculoDto> CriarAsync(CriarVeiculoDto dto)
    {
        var placa = NormalizarPlaca(dto.Placa);

        _ = await _clienteRepository.ObterPorIdAsync(dto.ClienteId)
            ?? throw new DomainException("Cliente não encontrado.");

        var veiculoExistente =
            await _veiculoRepository.ObterPorPlacaAsync(placa);

        if (veiculoExistente is not null)
            throw new DomainException(
                "Já existe um veículo cadastrado com esta placa.");

        var veiculo = new Veiculo(
            dto.ClienteId,
            placa,
            dto.Chassi,
            dto.Marca,
            dto.Modelo,
            dto.Cor,
            dto.Ano,
            dto.Quilometragem,
            _currentUser.Id);

        await _veiculoRepository.AdicionarAsync(veiculo);

        return MapearParaDto(veiculo);
    }

    public async Task AtualizarAsync(Guid id, AtualizarVeiculoDto dto)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id)
            ?? throw new DomainException("Veículo não encontrado.");

        var placa = NormalizarPlaca(dto.Placa);

        var veiculoExistente =
            await _veiculoRepository.ObterPorPlacaAsync(placa);

        if (veiculoExistente is not null && veiculoExistente.Id != id)
            throw new DomainException(
                "Já existe outro veículo cadastrado com esta placa.");

        veiculo.Atualizar(
            placa,
            dto.Chassi,
            dto.Marca,
            dto.Modelo,
            dto.Cor,
            dto.Ano,
            dto.Quilometragem,
            _currentUser.Id);

        await _veiculoRepository.AtualizarAsync(veiculo);
    }

    public async Task InativarAsync(Guid id)
    {
        var veiculo = await _veiculoRepository.ObterPorIdAsync(id) ?? throw new DomainException("Veículo não encontrado.");
        veiculo.Desativar(_currentUser.Id);

        await _veiculoRepository.AtualizarAsync(veiculo);
    }

    private static VeiculoDto MapearParaDto(Veiculo veiculo)
    {
        return new VeiculoDto
        {
            Id = veiculo.Id,
            ClienteId = veiculo.ClienteId,
            Placa = veiculo.Placa,
            Chassi = veiculo.Chassi,
            Marca = veiculo.Marca,
            Modelo = veiculo.Modelo,
            Cor = veiculo.Cor,
            Ano = veiculo.Ano,
            Quilometragem = veiculo.Quilometragem,
            Ativo = veiculo.Ativo
        };
    }

    private static string NormalizarPlaca(string placa)
    {
        return placa.Trim().Replace("-", "").ToUpperInvariant();
    }
}
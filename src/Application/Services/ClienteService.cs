using Application.DTOs;
using Application.DTOs.Cliente;
using Application.Interfaces;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.ValueObjects;

namespace Application.Services;

public class ClienteService(IClienteRepository clienteRepository, ICurrentUser currentUser) : IClienteService
{
    private readonly IClienteRepository _clienteRepository = clienteRepository;
    private readonly ICurrentUser _currentUser = currentUser;
    private const string ClienteNaoEncontrado = "Cliente não encontrado.";

    public async Task<ClienteDto> ObterPorIdAsync(Guid id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id) ?? throw new DomainException(ClienteNaoEncontrado);
        return MapearParaDto(cliente);
    }

    public async Task<IEnumerable<ClienteDto>> ObterTodosAsync()
    {
        var clientes = await _clienteRepository.ObterTodosAsync();

        return clientes.Select(MapearParaDto);
    }

    public async Task<ClienteDto> ObterPorDocumentoAsync(string documento)
    {
        var cliente = await _clienteRepository.ObterPorDocumentoAsync(documento) ?? throw new DomainException(ClienteNaoEncontrado);
        return MapearParaDto(cliente);
    }

    public async Task<ClienteDto> CriarAsync(CriarClienteDto dto)
    {

        var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(dto.Documento);

        if (clienteExistente is not null) throw new DomainException("Já existe um cliente cadastrado com este documento.");

        var endereco = CriarEndereco(dto.Endereco);

        var cliente = new Cliente(
            dto.Nome,
            dto.Documento,
            dto.TipoPessoa,
            dto.Telefone,
            dto.Email,
            _currentUser.Id,
            endereco);

        await _clienteRepository.AdicionarAsync(cliente);

        return MapearParaDto(cliente);
    }

    public async Task AtualizarAsync(Guid id, AtualizarClienteDto dto)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id) ?? throw new DomainException(ClienteNaoEncontrado);

        var endereco = CriarEndereco(dto.Endereco);

        cliente.Atualizar(
            dto.Nome,
            dto.TipoPessoa,
            dto.Telefone,
            dto.Email,
            endereco,
            _currentUser.Id);

        await _clienteRepository.AtualizarAsync(cliente);
    }

    public async Task InativarAsync(Guid id)
    {
        var cliente = await _clienteRepository.ObterPorIdAsync(id) ?? throw new DomainException(ClienteNaoEncontrado);

        cliente.Desativar(_currentUser.Id);

        await _clienteRepository.AtualizarAsync(cliente);
    }

    private static Endereco? CriarEndereco(EnderecoDto? dto)
    {
        if (dto is null)
            return null;

        return new Endereco(
            dto.Logradouro,
            dto.Numero,
            dto.Complemento,
            dto.Bairro,
            dto.Cidade,
            dto.Estado,
            dto.Cep);
    }

    private static ClienteDto MapearParaDto(Cliente cliente)
    {
        var endereco = cliente.Endereco;
        return new ClienteDto
        {
            Id = cliente.Id,
            Nome = cliente.Nome,
            Documento = cliente.Documento,
            TipoPessoa = cliente.TipoPessoa,
            Telefone = cliente.Telefone,
            Email = cliente.Email,
            Ativo = cliente.Ativo,
            Endereco = endereco is not null ? new EnderecoDto
            {
                Logradouro = endereco.Logradouro,
                Numero = endereco.Numero,
                Complemento = endereco.Complemento,
                Bairro = endereco.Bairro,
                Cidade = endereco.Cidade,
                Estado = endereco.Estado,
                Cep = endereco.Cep
            } : null
        };
    }
}
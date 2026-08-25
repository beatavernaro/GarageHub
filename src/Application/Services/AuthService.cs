using Application.DTOs.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Security;
using Application.Interfaces.Services;
using Domain.Exceptions;

namespace Application.Services;

public class AuthService(
    IUsuarioRepository usuarioRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
    : IAuthService
{
    private readonly IUsuarioRepository _usuarioRepository =
        usuarioRepository;

    private readonly IPasswordHasher _passwordHasher =
        passwordHasher;

    private readonly ITokenService _tokenService =
        tokenService;

    public async Task<LoginResponseDto> LoginAsync(
        LoginDto dto)
    {
        var email =
            dto.Email.Trim().ToLowerInvariant();

        var usuario =
            await _usuarioRepository
                .ObterPorEmailAsync(email);

        if (usuario is null ||
            !usuario.Ativo ||
            !_passwordHasher.Verificar(
                dto.Senha,
                usuario.SenhaHash))
        {
            throw new DomainException(
                "E-mail ou senha inválidos.");
        }

        var expiracao =
            DateTime.UtcNow.AddHours(8);

        var token =
            _tokenService.GerarToken(
                usuario,
                expiracao);

        return new LoginResponseDto
        {
            Token = token,
            ExpiraEm = expiracao,
            Nome = usuario.Nome,
            Email = usuario.Email
        };
    }
}
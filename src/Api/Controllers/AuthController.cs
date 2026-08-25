using Application.DTOs.Auth;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GarageHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    IAuthService authService)
    : ControllerBase
{
    private readonly IAuthService _authService =
        authService;

    [AllowAnonymous]
    [HttpPost("login")]
    [EndpointSummary("Realiza login administrativo")]
    [EndpointDescription(
        "Autentica o usuário e retorna um token JWT.")]
    [ProducesResponseType(
        typeof(LoginResponseDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginResponseDto>>
        Login(
            [FromBody] LoginDto dto)
    {
        var resultado =
            await _authService.LoginAsync(dto);

        return Ok(resultado);
    }
}
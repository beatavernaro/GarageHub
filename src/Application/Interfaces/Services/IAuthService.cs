using Application.DTOs.Auth;

namespace Application.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(
        LoginDto dto);
}
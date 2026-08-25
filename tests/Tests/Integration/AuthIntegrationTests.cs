using System.Net;
using System.Net.Http.Json;
using Application.DTOs.Auth;
using FluentAssertions;

namespace Tests.Integration;

public class AuthIntegrationTests
    : IClassFixture<GarageHubWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_Com_Credenciais_Validas_Deve_Retornar_Token()
    {
        var dto = new LoginDto
        {
            Email = "admin@garagehub.com",
            Senha = "Admin@123"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Auth/login",
                dto);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var resultado =
            await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        resultado.Should().NotBeNull();

        resultado!.Token
            .Should()
            .NotBeNullOrWhiteSpace();
    }
}
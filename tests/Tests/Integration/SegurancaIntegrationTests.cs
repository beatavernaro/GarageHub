using System.Net;
using System.Net.Http.Headers;
using FluentAssertions;

namespace Tests.Integration;

public class SegurancaIntegrationTests
    : IClassFixture<GarageHubWebApplicationFactory>
{
    private readonly HttpClient _client;

    public SegurancaIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Endpoint_Administrativo_Sem_Token_Deve_Retornar_Unauthorized()
    {
        _client.DefaultRequestHeaders.Authorization = null;

        var response =
            await _client.GetAsync(
                "/api/Clientes");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Endpoint_Administrativo_Com_Token_Invalido_Deve_Retornar_Unauthorized()
    {
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                "token-invalido");

        var response =
            await _client.GetAsync(
                "/api/Clientes");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Unauthorized);
    }
}
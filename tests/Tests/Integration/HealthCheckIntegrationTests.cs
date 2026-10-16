using System.Net;
using FluentAssertions;

namespace Tests.Integration;

public class HealthCheckIntegrationTests
    : IClassFixture<GarageHubWebApplicationFactory>
{
    private readonly HttpClient _client;

    public HealthCheckIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Liveness_Deve_Retornar_Ok_Sem_Autenticacao()
    {
        var response =
            await _client.GetAsync("/health/live");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Readiness_Com_Banco_Disponivel_Deve_Retornar_Ok()
    {
        var response =
            await _client.GetAsync("/health/ready");

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var conteudo =
            await response.Content.ReadAsStringAsync();

        conteudo.Should().Be("Healthy");
    }
}

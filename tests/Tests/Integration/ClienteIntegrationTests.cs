using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Application.DTOs.Cliente;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Integration;

public class ClienteIntegrationTests
    : IClassFixture<GarageHubWebApplicationFactory>
{
    private readonly HttpClient _client;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters =
        {
            new JsonStringEnumConverter()
        }
    };

    public ClienteIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Fazer_Login_Criar_Cliente_E_Consultar_Por_Id()
    {
        // Login
        var login = new LoginDto
        {
            Email = "admin@garagehub.com",
            Senha = "Admin@123"
        };

        var loginResponse =
            await _client.PostAsJsonAsync(
                "/api/Auth/login",
                login);

        loginResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var loginResult =
            await loginResponse.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        loginResult.Should().NotBeNull();

        loginResult!.Token
            .Should()
            .NotBeNullOrWhiteSpace();

        // Configura JWT
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        // Cria cliente
        var cpf = GerarCpfValido();

        var novoCliente = new CriarClienteDto
        {
            Nome = "Cliente Integração",
            Documento = cpf,
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "11999999999",
            Email = $"integracao-{Guid.NewGuid():N}@teste.com",
            Endereco = null
        };

        var criarResponse =
            await _client.PostAsJsonAsync(
                "/api/Clientes",
                novoCliente);

        criarResponse.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var clienteCriado =
            await criarResponse.Content
                .ReadFromJsonAsync<ClienteDto>(
                    JsonOptions);

        clienteCriado.Should().NotBeNull();

        clienteCriado!.Id
            .Should()
            .NotBeEmpty();

        clienteCriado.Nome
            .Should()
            .Be("Cliente Integração");

        clienteCriado.Documento
            .Should()
            .Be(cpf);

        // Consulta cliente criado
        var consultarResponse =
            await _client.GetAsync(
                $"/api/Clientes/{clienteCriado.Id}");

        consultarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var clienteConsultado =
            await consultarResponse.Content
                .ReadFromJsonAsync<ClienteDto>(
                    JsonOptions);

        clienteConsultado.Should().NotBeNull();

        clienteConsultado!.Id
            .Should()
            .Be(clienteCriado.Id);

        clienteConsultado.Nome
            .Should()
            .Be("Cliente Integração");

        clienteConsultado.Documento
            .Should()
            .Be(cpf);

        clienteConsultado.Email
            .Should()
            .Be(novoCliente.Email);
    }

    private static string GerarCpfValido()
    {
        var random = new Random();
        var numeros = new int[11];

        for (var i = 0; i < 9; i++)
            numeros[i] = random.Next(0, 10);

        var soma = 0;

        for (var i = 0; i < 9; i++)
            soma += numeros[i] * (10 - i);

        var resto = soma % 11;

        numeros[9] =
            resto < 2
                ? 0
                : 11 - resto;

        soma = 0;

        for (var i = 0; i < 10; i++)
            soma += numeros[i] * (11 - i);

        resto = soma % 11;

        numeros[10] =
            resto < 2
                ? 0
                : 11 - resto;

        return string.Concat(numeros);
    }
}
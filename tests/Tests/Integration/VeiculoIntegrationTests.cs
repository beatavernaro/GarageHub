using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Application.DTOs.Cliente;
using Application.DTOs.Veiculo;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Integration;

public class VeiculoIntegrationTests
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

    public VeiculoIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Fazer_Login_Criar_Cliente_Criar_Veiculo_E_Consultar_Por_Id()
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
        loginResult!.Token.Should().NotBeNullOrWhiteSpace();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                loginResult.Token);

        // Cria cliente
        var cpf = GerarCpfValido();

        var novoCliente = new CriarClienteDto
        {
            Nome = "Cliente Veículo Integração",
            Documento = cpf,
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "11999999999",
            Email = $"veiculo-{Guid.NewGuid():N}@teste.com",
            Endereco = null
        };

        var clienteResponse =
            await _client.PostAsJsonAsync(
                "/api/Clientes",
                novoCliente);

        clienteResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var cliente =
            await clienteResponse.Content
                .ReadFromJsonAsync<ClienteDto>(
                    JsonOptions);

        cliente.Should().NotBeNull();

        // Cria veículo
        var placa = GerarPlaca();

        var novoVeiculo = new CriarVeiculoDto
        {
            ClienteId = cliente!.Id,
            Placa = placa,
            Chassi = null,
            Marca = "Volkswagen",
            Modelo = "Gol",
            Cor = "Prata",
            Ano = 2020,
            Quilometragem = 50000
        };

        var veiculoResponse =
            await _client.PostAsJsonAsync(
                "/api/Veiculos",
                novoVeiculo);

        veiculoResponse.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var veiculoCriado =
            await veiculoResponse.Content
                .ReadFromJsonAsync<VeiculoDto>(
                    JsonOptions);

        veiculoCriado.Should().NotBeNull();

        veiculoCriado!.Id
            .Should()
            .NotBeEmpty();

        veiculoCriado.ClienteId
            .Should()
            .Be(cliente.Id);

        veiculoCriado.Placa
            .Should()
            .Be(placa);

        veiculoCriado.Marca
            .Should()
            .Be("Volkswagen");

        veiculoCriado.Modelo
            .Should()
            .Be("Gol");

        // Consulta veículo
        var consultarResponse =
            await _client.GetAsync(
                $"/api/Veiculos/{veiculoCriado.Id}");

        consultarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var veiculoConsultado =
            await consultarResponse.Content
                .ReadFromJsonAsync<VeiculoDto>(
                    JsonOptions);

        veiculoConsultado.Should().NotBeNull();

        veiculoConsultado!.Id
            .Should()
            .Be(veiculoCriado.Id);

        veiculoConsultado.ClienteId
            .Should()
            .Be(cliente.Id);

        veiculoConsultado.Placa
            .Should()
            .Be(placa);

        veiculoConsultado.Marca
            .Should()
            .Be("Volkswagen");

        veiculoConsultado.Modelo
            .Should()
            .Be("Gol");
    }

    private static string GerarPlaca()
    {
        var random = new Random();

        var numero1 = random.Next(0, 10);
        var letra = (char)random.Next('A', 'Z' + 1);
        var numero2 = random.Next(0, 10);
        var numero3 = random.Next(0, 10);

        return $"INT{numero1}{letra}{numero2}{numero3}";
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
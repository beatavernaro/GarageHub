using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Application.DTOs.Cliente;
using Application.DTOs.Orcamento;
using Application.DTOs.Veiculo;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Integration;

public class OrcamentoIntegrationTests
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

    public OrcamentoIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Criar_Orcamento_E_Consultar_Por_Id()
    {
        await AutenticarAsync();

        var cliente = await CriarClienteAsync();
        var veiculo = await CriarVeiculoAsync(cliente.Id);

        var dto = new CriarOrcamentoDto
        {
            ClienteId = cliente.Id,
            VeiculoId = veiculo.Id
        };

        var criarResponse =
            await _client.PostAsJsonAsync(
                "/api/Orcamentos",
                dto);

        criarResponse.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var orcamentoCriado =
            await criarResponse.Content
                .ReadFromJsonAsync<OrcamentoDto>(
                    JsonOptions);

        orcamentoCriado.Should().NotBeNull();

        orcamentoCriado!.Id
            .Should()
            .NotBeEmpty();

        orcamentoCriado.ClienteId
            .Should()
            .Be(cliente.Id);

        orcamentoCriado.VeiculoId
            .Should()
            .Be(veiculo.Id);

        orcamentoCriado.Status
            .Should()
            .Be(StatusOrcamento.EmElaboracao);

        var consultarResponse =
            await _client.GetAsync(
                $"/api/Orcamentos/{orcamentoCriado.Id}");

        consultarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var orcamentoConsultado =
            await consultarResponse.Content
                .ReadFromJsonAsync<OrcamentoDto>(
                    JsonOptions);

        orcamentoConsultado.Should().NotBeNull();

        orcamentoConsultado!.Id
            .Should()
            .Be(orcamentoCriado.Id);

        orcamentoConsultado.ClienteId
            .Should()
            .Be(cliente.Id);

        orcamentoConsultado.VeiculoId
            .Should()
            .Be(veiculo.Id);
    }

    private async Task AutenticarAsync()
    {
        var login = new LoginDto
        {
            Email = "admin@garagehub.com",
            Senha = "Admin@123"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Auth/login",
                login);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var resultado =
            await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        resultado.Should().NotBeNull();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                resultado!.Token);
    }

    private async Task<ClienteDto> CriarClienteAsync()
    {
        var dto = new CriarClienteDto
        {
            Nome = "Cliente Orçamento Integração",
            Documento = GerarCpfValido(),
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "11999999999",
            Email = $"orcamento-{Guid.NewGuid():N}@teste.com",
            Endereco = null
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Clientes",
                dto);

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var cliente =
            await response.Content
                .ReadFromJsonAsync<ClienteDto>(
                    JsonOptions);

        cliente.Should().NotBeNull();

        return cliente!;
    }

    private async Task<VeiculoDto> CriarVeiculoAsync(
        Guid clienteId)
    {
        var dto = new CriarVeiculoDto
        {
            ClienteId = clienteId,
            Placa = GerarPlaca(),
            Chassi = null,
            Marca = "Volkswagen",
            Modelo = "Gol",
            Cor = "Prata",
            Ano = 2020,
            Quilometragem = 50000
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Veiculos",
                dto);

        response.IsSuccessStatusCode
            .Should()
            .BeTrue();

        var veiculo =
            await response.Content
                .ReadFromJsonAsync<VeiculoDto>(
                    JsonOptions);

        veiculo.Should().NotBeNull();

        return veiculo!;
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
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Application.DTOs.Cliente;
using Application.DTOs.Orcamento;
using Application.DTOs.OrdemServico;
using Application.DTOs.Servico;
using Application.DTOs.Veiculo;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Integration;

public class FluxoOrdemServicoIntegrationTests
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

    public FluxoOrdemServicoIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Criar_E_Aprovar_Orcamento_Gerando_Ordem_Servico()
    {
        await AutenticarAsync();

        var cliente =
            await CriarClienteAsync();

        var veiculo =
            await CriarVeiculoAsync(
                cliente.Id);

        var servico =
            await CriarServicoAsync();

        // orçamento
        var criarOrcamentoResponse =
            await _client.PostAsJsonAsync(
                "/api/Orcamentos",
                new CriarOrcamentoDto
                {
                    ClienteId = cliente.Id,
                    VeiculoId = veiculo.Id
                });

        criarOrcamentoResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var orcamento =
            await criarOrcamentoResponse.Content
                .ReadFromJsonAsync<OrcamentoDto>(
                    JsonOptions);

        orcamento.Should().NotBeNull();

        // adiciona serviço
        var adicionarItem =
            new AdicionarOrcamentoItemDto
            {
                ServicoId = servico.Id,
                ItemEstoqueId = null,
                Quantidade = 1,
                ValorUnitario = servico.Preco
            };

        var itemResponse =
            await _client.PostAsJsonAsync(
                $"/api/Orcamentos/{orcamento!.Id}/itens",
                adicionarItem);

        itemResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        // envia para cliente
        var aguardandoResponse =
            await _client.PostAsync(
                $"/api/Orcamentos/{orcamento.Id}/aguardando-cliente",
                null);

        aguardandoResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        // aprovação é pública
        var aprovarResponse =
            await _client.PostAsync(
            $"/api/Orcamentos/{orcamento.Id}/aprovar",
            null);

        aprovarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var resultado =
            await aprovarResponse.Content
                .ReadFromJsonAsync<ResultadoAprovacaoOrcamentoDto>(
                    JsonOptions);

        resultado.Should().NotBeNull();
        resultado!.OrcamentoId.Should()
            .Be(orcamento.Id);

        // autentica novamente para consultar OS administrativa
        await AutenticarAsync();

        var ordensResponse =
            await _client.GetAsync(
                "/api/OrdensServico");

        ordensResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.OK);

        var ordens =
            await ordensResponse.Content
                .ReadFromJsonAsync<List<OrdemServicoDto>>(
                    JsonOptions);

        ordens.Should().NotBeNull();

        var ordem =
            ordens!
                .SingleOrDefault(
                    x => x.OrcamentoId == orcamento.Id);

        ordem.Should().NotBeNull();

        ordem!.ClienteId.Should()
            .Be(cliente.Id);

        ordem.VeiculoId.Should()
            .Be(veiculo.Id);

        ordem.Servicos.Should()
            .ContainSingle();

        ordem.Servicos.Single()
            .ServicoId.Should()
            .Be(servico.Id);
    }

    private async Task AutenticarAsync()
    {
        var response =
            await _client.PostAsJsonAsync(
                "/api/Auth/login",
                new LoginDto
                {
                    Email = "admin@garagehub.com",
                    Senha = "Admin@123"
                });

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
            Nome = "Cliente Fluxo OS",
            Documento = GerarCpfValido(),
            TipoPessoa = TipoPessoa.Fisica,
            Telefone = "11999999999",
            Email = $"os-{Guid.NewGuid():N}@teste.com"
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Clientes",
                dto);

        var cliente =
            await response.Content
                .ReadFromJsonAsync<ClienteDto>(
                    JsonOptions);

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
            Marca = "Honda",
            Modelo = "Civic",
            Cor = "Preto",
            Ano = 2021,
            Quilometragem = 30000
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Veiculos",
                dto);

        var veiculo =
            await response.Content
                .ReadFromJsonAsync<VeiculoDto>(
                    JsonOptions);

        return veiculo!;
    }

    private async Task<ServicoDto> CriarServicoAsync()
    {
        var codigo =
            $"SER{Random.Shared.Next(1000, 9999)}";

        var dto = new CriarServicoDto
        {
            CodigoInterno = codigo,
            Nome = "Serviço Integração",
            Descricao = "Serviço utilizado no fluxo de OS",
            Preco = 200m
        };

        var response =
            await _client.PostAsJsonAsync(
                "/api/Servicos",
                dto);

        response.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var servico =
            await response.Content
                .ReadFromJsonAsync<ServicoDto>(
                    JsonOptions);

        return servico!;
    }

    private static string GerarPlaca()
    {
        var random = new Random();

        return $"TST{random.Next(0, 10)}" +
               $"{(char)random.Next('A', 'Z' + 1)}" +
               $"{random.Next(0, 10)}" +
               $"{random.Next(0, 10)}";
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
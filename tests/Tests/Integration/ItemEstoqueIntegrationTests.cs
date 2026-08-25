using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Application.DTOs.Auth;
using Application.DTOs.ItemEstoque;
using Domain.Enums;
using FluentAssertions;

namespace Tests.Integration;

public class ItemEstoqueIntegrationTests
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

    public ItemEstoqueIntegrationTests(
        GarageHubWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Deve_Criar_Item_Adicionar_E_Remover_Estoque()
    {
        await AutenticarAsync();

        var codigo =
            $"PEC{Random.Shared.Next(1000, 9999)}";

        var dto = new CriarItemEstoqueDto
        {
            CodigoInterno = codigo,
            Nome = "Pastilha Integração",
            Descricao = "Item criado pelo teste",
            Tipo = TipoItemEstoque.Peca,
            Preco = 150m,
            Estoque = 10
        };

        var criarResponse =
            await _client.PostAsJsonAsync(
                "/api/ItensEstoque",
                dto);

        criarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.Created);

        var item =
            await criarResponse.Content
                .ReadFromJsonAsync<ItemEstoqueDto>(
                    JsonOptions);

        item.Should().NotBeNull();
        item!.Estoque.Should().Be(10);

        // adiciona 5
        var adicionarResponse =
            await _client.PatchAsJsonAsync(
                $"/api/ItensEstoque/{item.Id}/adicionar-estoque",
                5);

        adicionarResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var consultar1 =
            await _client.GetFromJsonAsync<ItemEstoqueDto>(
                $"/api/ItensEstoque/{item.Id}",
                JsonOptions);

        consultar1!.Estoque.Should().Be(15);

        // remove 4
        var removerResponse =
            await _client.PatchAsJsonAsync(
                $"/api/ItensEstoque/{item.Id}/remover-estoque",
                4);

        removerResponse.StatusCode
            .Should()
            .Be(HttpStatusCode.NoContent);

        var consultar2 =
            await _client.GetFromJsonAsync<ItemEstoqueDto>(
                $"/api/ItensEstoque/{item.Id}",
                JsonOptions);

        consultar2!.Estoque.Should().Be(11);
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

        var resultado =
            await response.Content
                .ReadFromJsonAsync<LoginResponseDto>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue(
                "Bearer",
                resultado!.Token);
    }
}
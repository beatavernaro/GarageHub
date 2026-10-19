using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Infrastructure.Notifications;
using Microsoft.Extensions.Configuration;

namespace GarageHub.Tests.Infrastructure.Notifications;

public class SmtpEmailSenderTests
{
    [Fact]
    public async Task EnviarAsync_Deve_Respeitar_Timeout_Quando_Servidor_Nao_Responde()
    {
        // Aceita a conexão, mas nunca envia a saudação SMTP
        using var servidor =
            new TcpListener(IPAddress.Loopback, 0);

        servidor.Start();

        var porta =
            ((IPEndPoint)servidor.LocalEndpoint).Port;

        var sender =
            new SmtpEmailSender(
                CriarConfiguracao(porta, timeoutSegundos: 1));

        var cronometro = Stopwatch.StartNew();

        var acao =
            async () =>
                await sender.EnviarAsync(
                    "cliente@teste.com",
                    "Assunto",
                    "Corpo");

        await acao.Should()
            .ThrowAsync<Exception>();

        cronometro.Elapsed.Should()
            .BeLessThan(TimeSpan.FromSeconds(10));
    }

    [Fact]
    public async Task EnviarAsync_Deve_Lancar_Excecao_Quando_Host_Nao_Configurado()
    {
        var sender =
            new SmtpEmailSender(
                new ConfigurationBuilder().Build());

        var acao =
            async () =>
                await sender.EnviarAsync(
                    "cliente@teste.com",
                    "Assunto",
                    "Corpo");

        await acao.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("SMTP Host não configurado.");
    }

    private static IConfiguration CriarConfiguracao(
        int porta,
        int timeoutSegundos)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["Smtp:Host"] = "127.0.0.1",
                    ["Smtp:Port"] = porta.ToString(),
                    ["Smtp:From"] = "nao-responda@garagehub.local",
                    ["Smtp:TimeoutSeconds"] = timeoutSegundos.ToString()
                })
            .Build();
    }
}

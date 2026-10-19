using System.Net.Mail;
using Application.Interfaces.Notifications;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Notifications;

public class SmtpEmailSender(
    IConfiguration configuration)
    : IEmailSender
{
    private const int TimeoutPadraoSegundos = 5;

    private readonly IConfiguration _configuration =
        configuration;

    public async Task EnviarAsync(
        string destinatario,
        string assunto,
        string corpo)
    {
        var host =
            _configuration["Smtp:Host"]
            ?? throw new InvalidOperationException(
                "SMTP Host não configurado.");

        var porta =
            ObterInteiro("Smtp:Port", 25);

        var remetente =
            _configuration["Smtp:From"]
            ?? throw new InvalidOperationException(
                "SMTP From não configurado.");

        var timeout =
            TimeSpan.FromSeconds(
                ObterInteiro(
                    "Smtp:TimeoutSeconds",
                    TimeoutPadraoSegundos));

        using var mensagem =
            new MailMessage(
                new MailAddress(remetente),
                new MailAddress(destinatario))
            {
                Subject = assunto,
                Body = corpo,
                IsBodyHtml = false
            };

        using var smtpClient =
            new SmtpClient(host, porta)
            {
                Timeout = (int)timeout.TotalMilliseconds
            };

        // O Timeout do SmtpClient não se aplica ao envio assíncrono:
        // o token limita o tempo que a requisição aguarda o servidor SMTP
        using var cancellation =
            new CancellationTokenSource(timeout);

        await smtpClient.SendMailAsync(
            mensagem,
            cancellation.Token);
    }

    private int ObterInteiro(
        string chave,
        int padrao)
    {
        return int.TryParse(
            _configuration[chave],
            out var valor)
            ? valor
            : padrao;
    }
}

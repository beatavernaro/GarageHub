using System.Collections.Concurrent;
using Application.Interfaces.Notifications;

namespace Tests.Integration;

// Substitui o envio SMTP nos testes de integração: as mensagens
// ficam em memória para que os testes possam verificá-las
public class FakeEmailSender : IEmailSender
{
    public ConcurrentQueue<EmailEnviado> Enviados { get; } = new();

    public Task EnviarAsync(
        string destinatario,
        string assunto,
        string corpo)
    {
        Enviados.Enqueue(
            new EmailEnviado(
                destinatario,
                assunto,
                corpo));

        return Task.CompletedTask;
    }
}

public record EmailEnviado(
    string Destinatario,
    string Assunto,
    string Corpo);

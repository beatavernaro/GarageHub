namespace Application.Interfaces.Notifications;

public interface IEmailSender
{
    Task EnviarAsync(
        string destinatario,
        string assunto,
        string corpo);
}

using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ReservaPeriferico.Application.Configuration;
using ReservaPeriferico.Application.Interfaces;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class EmailNotificationService : INotificationService
{
    private readonly EmailSettings _emailSettings;
    private readonly DatabaseEmailSettings _databaseEmailSettings;
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(
        IOptions<EmailSettings> emailSettings,
        DatabaseEmailSettings databaseEmailSettings,
        ILogger<EmailNotificationService> logger)
    {
        _emailSettings = emailSettings.Value;
        _databaseEmailSettings = databaseEmailSettings;
        _logger = logger;
    }

    public bool CanHandle(NotificationType type) => type == NotificationType.Email;

    public async Task<bool> SendAsync(NotificationMessage message)
    {
        try
        {
            if (message.Type != NotificationType.Email)
            {
                _logger.LogWarning("Tentativa de enviar mensagem não-email através do EmailNotificationService");
                return false;
            }

            var emailSettings = await _databaseEmailSettings.GetEmailSettingsAsync();
            using var client = CreateSmtpClient(emailSettings);
            using var mailMessage = CreateMailMessage(message, emailSettings);

            await client.SendMailAsync(mailMessage);

            _logger.LogInformation("Email enviado com sucesso para {To}", message.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar email para {To}: {Message}", message.To, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendAsync(IEnumerable<NotificationMessage> messages)
    {
        var results = new List<bool>();
        
        foreach (var message in messages.Where(m => m.Type == NotificationType.Email))
        {
            var result = await SendAsync(message);
            results.Add(result);
        }

        return results.All(r => r);
    }

    private SmtpClient CreateSmtpClient(EmailSettings settings)
    {
        return new SmtpClient(settings.SmtpServer, settings.SmtpPort)
        {
            Credentials = new NetworkCredential(settings.Username, settings.Password),
            EnableSsl = settings.EnableSsl,
            Timeout = settings.Timeout
        };
    }

    private MailMessage CreateMailMessage(NotificationMessage message, EmailSettings settings)
    {
        var mailMessage = new MailMessage
        {
            From = new MailAddress(settings.FromEmail, settings.FromName),
            Subject = message.Subject,
            Body = message.Body,
            IsBodyHtml = message.IsHtml
        };

        // Adicionar destinatários
        foreach (var email in message.To.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            mailMessage.To.Add(email.Trim());
        }

        if (!string.IsNullOrEmpty(message.Cc))
        {
            foreach (var email in message.Cc.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.CC.Add(email.Trim());
            }
        }

        if (!string.IsNullOrEmpty(message.Bcc))
        {
            foreach (var email in message.Bcc.Split(';', StringSplitOptions.RemoveEmptyEntries))
            {
                mailMessage.Bcc.Add(email.Trim());
            }
        }

        return mailMessage;
    }
}


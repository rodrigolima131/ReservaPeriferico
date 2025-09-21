using Microsoft.Extensions.Logging;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class SmsNotificationService : INotificationService
{
    private readonly ILogger<SmsNotificationService> _logger;

    public SmsNotificationService(ILogger<SmsNotificationService> logger)
    {
        _logger = logger;
    }

    public bool CanHandle(NotificationType type) => type == NotificationType.Sms;

    public async Task<bool> SendAsync(NotificationMessage message)
    {
        try
        {
            if (message.Type != NotificationType.Sms)
            {
                _logger.LogWarning("Tentativa de enviar mensagem não-SMS através do SmsNotificationService");
                return false;
            }

            // Simular envio de SMS
            await Task.Delay(100); // Simular latência de rede

            _logger.LogInformation("SMS enviado com sucesso para {To}", message.To);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar SMS para {To}: {Message}", message.To, ex.Message);
            return false;
        }
    }

    public async Task<bool> SendAsync(IEnumerable<NotificationMessage> messages)
    {
        var results = new List<bool>();
        
        foreach (var message in messages.Where(m => m.Type == NotificationType.Sms))
        {
            var result = await SendAsync(message);
            results.Add(result);
        }

        return results.All(r => r);
    }
}


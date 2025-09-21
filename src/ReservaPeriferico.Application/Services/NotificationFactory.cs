using Microsoft.Extensions.Logging;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class NotificationFactory : INotificationFactory
{
    private readonly IEnumerable<INotificationService> _notificationServices;
    private readonly ILogger<NotificationFactory> _logger;

    public NotificationFactory(
        IEnumerable<INotificationService> notificationServices,
        ILogger<NotificationFactory> logger)
    {
        _notificationServices = notificationServices;
        _logger = logger;
    }

    public INotificationService GetService(NotificationType type)
    {
        var service = _notificationServices.FirstOrDefault(s => s.CanHandle(type));
        
        if (service == null)
        {
            _logger.LogWarning("Nenhum serviço de notificação encontrado para o tipo {NotificationType}", type);
            throw new InvalidOperationException($"Nenhum serviço de notificação encontrado para o tipo {type}");
        }

        return service;
    }

    public IEnumerable<INotificationService> GetAllServices()
    {
        return _notificationServices;
    }
}


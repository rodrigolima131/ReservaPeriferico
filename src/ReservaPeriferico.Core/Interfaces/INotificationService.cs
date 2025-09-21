using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Core.Interfaces;

public interface INotificationService
{
    Task<bool> SendAsync(NotificationMessage message);
    Task<bool> SendAsync(IEnumerable<NotificationMessage> messages);
    bool CanHandle(NotificationType type);
}

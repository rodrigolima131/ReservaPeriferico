using ReservaPeriferico.Core.Enums;

namespace ReservaPeriferico.Core.Interfaces;

public interface INotificationFactory
{
    INotificationService GetService(NotificationType type);
    IEnumerable<INotificationService> GetAllServices();
}


using Hangfire;
using Microsoft.Extensions.Logging;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public interface IEmailJobService
{
    Task EnqueueEmailAsync(NotificationMessage message);
    Task EnqueueEmailsAsync(IEnumerable<NotificationMessage> messages);
}

public class EmailJobService : IEmailJobService
{
    private readonly INotificationFactory _notificationFactory;
    private readonly ILogger<EmailJobService> _logger;

    public EmailJobService(
        INotificationFactory notificationFactory,
        ILogger<EmailJobService> logger)
    {
        _notificationFactory = notificationFactory;
        _logger = logger;
    }

    public Task EnqueueEmailAsync(NotificationMessage message)
    {
        try
        {
            // Criar um job único para envio de email
            var jobId = BackgroundJob.Enqueue(() => SendEmailJob(message));
            
            _logger.LogInformation("Job de email enfileirado com ID: {JobId} para {Email}", jobId, message.To);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enfileirar job de email para {Email}: {Message}", message.To, ex.Message);
            throw;
        }
    }

    public Task EnqueueEmailsAsync(IEnumerable<NotificationMessage> messages)
    {
        try
        {
            var jobIds = new List<string>();
            
            foreach (var message in messages)
            {
                var jobId = BackgroundJob.Enqueue(() => SendEmailJob(message));
                jobIds.Add(jobId);
            }
            
            _logger.LogInformation("Jobs de email enfileirados: {Count} jobs para envio em lote", jobIds.Count);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enfileirar jobs de email em lote: {Message}", ex.Message);
            throw;
        }
    }

    /// <summary>
    /// Job executado pelo Hangfire para envio de email
    /// Este método é chamado pelo Hangfire e não deve ser chamado diretamente
    /// </summary>
    [AutomaticRetry(Attempts = 3, DelaysInSeconds = new[] { 30, 60, 120 })]
    public async Task SendEmailJob(NotificationMessage message)
    {
        try
        {
            _logger.LogInformation("Iniciando envio de email para {Email}", message.To);

            var emailService = _notificationFactory.GetService(message.Type);
            var resultado = await emailService.SendAsync(message);

            if (resultado)
            {
                _logger.LogInformation("Email enviado com sucesso para {Email}", message.To);
            }
            else
            {
                var errorMessage = $"Falha no envio de email para {message.To}";
                _logger.LogError(errorMessage);
                throw new InvalidOperationException(errorMessage);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro no job de envio de email para {Email}: {Message}", 
                message.To, ex.Message);
            throw; // Re-throw para que o Hangfire possa fazer retry
        }
    }
}

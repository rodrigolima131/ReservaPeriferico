using Microsoft.Extensions.Logging;
using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Services.EmailTemplates;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;

namespace ReservaPeriferico.Application.Services;

public class ReservaNotificationService
{
    private readonly IEmailJobService _emailJobService;
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly ILogger<ReservaNotificationService> _logger;

    public ReservaNotificationService(
        IEmailJobService emailJobService,
        IEmailTemplateService emailTemplateService,
        ILogger<ReservaNotificationService> logger)
    {
        _emailJobService = emailJobService;
        _emailTemplateService = emailTemplateService;
        _logger = logger;
    }

    public async Task NotificarSolicitacaoReservaAsync(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico, UsuarioDto administradorEquipe)
    {
        try
        {
            var messages = new List<NotificationMessage>();

            var mensagemUsuario = new NotificationMessage
            {
                To = usuario.Email,
                Subject = "Solicitação de reserva enviada",
                Body = _emailTemplateService.GerarCorpoEmailSolicitacaoUsuario(reserva, periferico),
                Type = NotificationType.Email,
                IsHtml = true,
                Metadata = new Dictionary<string, object>
                {
                    ["ReservaId"] = reserva.Id,
                    ["UsuarioId"] = usuario.Id,
                    ["TipoNotificacao"] = "SolicitacaoReserva"
                }
            };
            messages.Add(mensagemUsuario);

            // Notificar o administrador da equipe
            var mensagemAdmin = new NotificationMessage
            {
                To = administradorEquipe.Email,
                Subject = "Nova solicitação de reserva pendente",
                Body = _emailTemplateService.GerarCorpoEmailSolicitacaoAdmin(reserva, usuario, periferico),
                Type = NotificationType.Email,
                IsHtml = true,
                Metadata = new Dictionary<string, object>
                {
                    ["ReservaId"] = reserva.Id,
                    ["UsuarioId"] = usuario.Id,
                    ["TipoNotificacao"] = "SolicitacaoReservaAdmin"
                }
            };
            messages.Add(mensagemAdmin);

            await _emailJobService.EnqueueEmailsAsync(messages);

            _logger.LogInformation("Notificações de solicitação de reserva enfileiradas com sucesso para reserva {ReservaId}", reserva.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificações de solicitação de reserva para reserva {ReservaId}: {Message}", reserva.Id, ex.Message);
        }
    }

    public async Task NotificarAprovacaoReservaAsync(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico, string status, string? motivoRejeicao = null)
    {
        try
        {
            var mensagem = new NotificationMessage
            {
                To = usuario.Email,
                Subject = status == "Aprovada" ? "Reserva Aprovada" : "Reserva Rejeitada",
                Body = _emailTemplateService.GerarCorpoEmailAprovacao(reserva, periferico, status, motivoRejeicao),
                Type = NotificationType.Email,
                IsHtml = true,
                Metadata = new Dictionary<string, object>
                {
                    ["ReservaId"] = reserva.Id,
                    ["UsuarioId"] = usuario.Id,
                    ["Status"] = status,
                    ["TipoNotificacao"] = "AprovacaoReserva"
                }
            };

            await _emailJobService.EnqueueEmailAsync(mensagem);

            _logger.LogInformation("Notificação de {Status} de reserva enfileirada com sucesso para reserva {ReservaId}", status, reserva.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de {Status} de reserva para reserva {ReservaId}: {Message}", status, reserva.Id, ex.Message);
        }
    }

    public async Task NotificarCancelamentoReservaAsync(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico, string status, string? motivoCancelamento = null)
    {
        try
        {
            var mensagem = new NotificationMessage
            {
                To = usuario.Email,
                Subject =  "Reserva cancelada",
                Body = _emailTemplateService.GerarCorpoEmailCancelamentoReserva(reserva, periferico, status, motivoCancelamento),
                Type = NotificationType.Email,
                IsHtml = true,
                Metadata = new Dictionary<string, object>
                {
                    ["ReservaId"] = reserva.Id,
                    ["UsuarioId"] = usuario.Id,
                    ["Status"] = status,
                    ["TipoNotificacao"] = "CancelamentoReserva"
                }
            };

            await _emailJobService.EnqueueEmailAsync(mensagem);

            _logger.LogInformation("Notificação de {Status} de reserva enfileirada com sucesso para reserva {ReservaId}", status, reserva.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao enviar notificação de {Status} de reserva para reserva {ReservaId}: {Message}", status, reserva.Id, ex.Message);
        }
    }

}


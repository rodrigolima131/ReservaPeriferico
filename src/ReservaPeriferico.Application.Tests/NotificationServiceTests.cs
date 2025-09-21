using Microsoft.Extensions.Logging;
using Moq;
using ReservaPeriferico.Application.DTOs;
using ReservaPeriferico.Application.Services;
using ReservaPeriferico.Core.Entities;
using ReservaPeriferico.Core.Enums;
using ReservaPeriferico.Core.Interfaces;
using Xunit;

namespace ReservaPeriferico.Application.Tests;

public class NotificationServiceTests
{
    private readonly Mock<INotificationFactory> _mockNotificationFactory;
    private readonly Mock<INotificationService> _mockEmailService;
    private readonly Mock<ILogger<ReservaNotificationService>> _mockLogger;
    private readonly ReservaNotificationService _notificationService;

    public NotificationServiceTests()
    {
        _mockNotificationFactory = new Mock<INotificationFactory>();
        _mockEmailService = new Mock<INotificationService>();
        _mockLogger = new Mock<ILogger<ReservaNotificationService>>();

        _mockEmailService.Setup(x => x.CanHandle(NotificationType.Email)).Returns(true);
        _mockNotificationFactory.Setup(x => x.GetService(NotificationType.Email)).Returns(_mockEmailService.Object);

        _notificationService = new ReservaNotificationService(_mockNotificationFactory.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task NotificarSolicitacaoReservaAsync_DeveEnviarNotificacoesParaUsuarioEAdmin()
    {
        // Arrange
        var reserva = new ReservaDto
        {
            Id = 1,
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddHours(2),
            Observacoes = "Teste de notificação"
        };

        var usuario = new UsuarioDto
        {
            Id = 1,
            Nome = "Usuário Teste",
            Email = "usuario@teste.com"
        };

        var periferico = new PerifericoDto
        {
            Id = 1,
            Nome = "Periférico Teste",
            Tipo = "Mouse",
            Marca = "Logitech",
            Modelo = "M185"
        };

        var admin = new UsuarioDto
        {
            Id = 2,
            Nome = "Admin Teste",
            Email = "admin@teste.com"
        };

        _mockEmailService.Setup(x => x.SendAsync(It.IsAny<IEnumerable<NotificationMessage>>()))
            .ReturnsAsync(true);

        // Act
        await _notificationService.NotificarSolicitacaoReservaAsync(reserva, usuario, periferico, admin);

        // Assert
        _mockEmailService.Verify(x => x.SendAsync(It.Is<IEnumerable<NotificationMessage>>(messages =>
            messages.Count() == 2 &&
            messages.Any(m => m.To == usuario.Email && m.Subject.Contains("Solicitação de Reserva Enviada")) &&
            messages.Any(m => m.To == admin.Email && m.Subject.Contains("Nova Solicitação de Reserva Pendente"))
        )), Times.Once);
    }

    [Fact]
    public async Task NotificarAprovacaoReservaAsync_DeveEnviarNotificacaoDeAprovacao()
    {
        // Arrange
        var reserva = new ReservaDto
        {
            Id = 1,
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddHours(2)
        };

        var usuario = new UsuarioDto
        {
            Id = 1,
            Nome = "Usuário Teste",
            Email = "usuario@teste.com"
        };

        var periferico = new PerifericoDto
        {
            Id = 1,
            Nome = "Periférico Teste"
        };

        _mockEmailService.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>()))
            .ReturnsAsync(true);

        // Act
        await _notificationService.NotificarAprovacaoReservaAsync(reserva, usuario, periferico, "Aprovada");

        // Assert
        _mockEmailService.Verify(x => x.SendAsync(It.Is<NotificationMessage>(m =>
            m.To == usuario.Email &&
            m.Subject == "Reserva Aprovada" &&
            m.Body.Contains("Parabéns! Sua solicitação de reserva foi aprovada")
        )), Times.Once);
    }

    [Fact]
    public async Task NotificarAprovacaoReservaAsync_DeveEnviarNotificacaoDeRejeicao()
    {
        // Arrange
        var reserva = new ReservaDto
        {
            Id = 1,
            DataInicio = DateTime.Now,
            DataFim = DateTime.Now.AddHours(2)
        };

        var usuario = new UsuarioDto
        {
            Id = 1,
            Nome = "Usuário Teste",
            Email = "usuario@teste.com"
        };

        var periferico = new PerifericoDto
        {
            Id = 1,
            Nome = "Periférico Teste"
        };

        var motivoRejeicao = "Periférico indisponível";

        _mockEmailService.Setup(x => x.SendAsync(It.IsAny<NotificationMessage>()))
            .ReturnsAsync(true);

        // Act
        await _notificationService.NotificarAprovacaoReservaAsync(reserva, usuario, periferico, "Rejeitada", motivoRejeicao);

        // Assert
        _mockEmailService.Verify(x => x.SendAsync(It.Is<NotificationMessage>(m =>
            m.To == usuario.Email &&
            m.Subject == "Reserva Rejeitada" &&
            m.Body.Contains("Infelizmente sua solicitação de reserva foi rejeitada") &&
            m.Body.Contains(motivoRejeicao)
        )), Times.Once);
    }

    [Fact]
    public async Task NotificarSolicitacaoReservaAsync_DeveTratarErroSemFalhar()
    {
        // Arrange
        var reserva = new ReservaDto { Id = 1 };
        var usuario = new UsuarioDto { Id = 1, Email = "usuario@teste.com" };
        var periferico = new PerifericoDto { Id = 1 };
        var admin = new UsuarioDto { Id = 2, Email = "admin@teste.com" };

        _mockEmailService.Setup(x => x.SendAsync(It.IsAny<IEnumerable<NotificationMessage>>()))
            .ThrowsAsync(new Exception("Erro de rede"));

        // Act & Assert - Não deve lançar exceção
        await _notificationService.NotificarSolicitacaoReservaAsync(reserva, usuario, periferico, admin);

        // Verificar que o serviço foi chamado mesmo com erro
        _mockEmailService.Verify(x => x.SendAsync(It.IsAny<IEnumerable<NotificationMessage>>()), Times.Once);
    }
}


# Sistema de Notificações - ReservaPeriferico

## Visão Geral

Este sistema implementa uma arquitetura robusta e extensível para notificações, permitindo o envio de diferentes tipos de notificações (Email, SMS, Push) de forma flexível e configurável.

## Arquitetura

### 1. Interfaces Base

- **`INotificationService`**: Interface base para todos os serviços de notificação
- **`INotificationFactory`**: Factory para obter a instância correta do serviço de notificação

### 2. Entidades

- **`NotificationMessage`**: Classe que contém todas as informações necessárias para uma notificação
- **`NotificationType`**: Enum que define os tipos de notificação suportados

### 3. Serviços Implementados

- **`EmailNotificationService`**: Implementação para envio de emails via SMTP
- **`SmsNotificationService`**: Implementação de exemplo para SMS
- **`ReservaNotificationService`**: Serviço específico para notificações de reserva
- **`NotificationFactory`**: Factory que gerencia todos os serviços de notificação

## Configuração

### Email Settings (appsettings.json)

```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "Username": "seu-email@gmail.com",
    "Password": "sua-senha-de-app",
    "EnableSsl": true,
    "FromEmail": "seu-email@gmail.com",
    "FromName": "Sistema de Reservas",
    "Timeout": 30000
  }
}
```

### Configuração de Serviços

Os serviços são registrados automaticamente no `ServiceCollectionExtensions.cs`:

```csharp
// Configurar EmailSettings
services.Configure<EmailSettings>(configuration.GetSection("EmailSettings"));

// Registrar serviços de notificação
services.AddScoped<INotificationFactory, NotificationFactory>();
services.AddScoped<ReservaNotificationService>();
services.AddScoped<INotificationService, EmailNotificationService>();
services.AddScoped<INotificationService, SmsNotificationService>();
```

## Uso

### 1. Notificações de Reserva

O sistema automaticamente envia notificações nos seguintes cenários:

- **Solicitação de Reserva**: Notifica o usuário solicitante e o administrador da equipe
- **Aprovação/Rejeição**: Notifica o usuário sobre o status da sua solicitação

### 2. Uso Direto dos Serviços

```csharp
// Obter o factory
var notificationFactory = serviceProvider.GetService<INotificationFactory>();

// Enviar email
var emailService = notificationFactory.GetService(NotificationType.Email);
var message = new NotificationMessage
{
    To = "usuario@exemplo.com",
    Subject = "Assunto do Email",
    Body = "Corpo do email",
    Type = NotificationType.Email,
    IsHtml = true
};

await emailService.SendAsync(message);
```

## Extensibilidade

### Adicionando Novos Tipos de Notificação

1. **Adicionar novo tipo no enum**:
```csharp
public enum NotificationType
{
    Email = 1,
    Sms = 2,
    PushNotification = 3,
    WhatsApp = 4  // Novo tipo
}
```

2. **Implementar o serviço**:
```csharp
public class WhatsAppNotificationService : INotificationService
{
    public bool CanHandle(NotificationType type) => type == NotificationType.WhatsApp;
    
    public async Task<bool> SendAsync(NotificationMessage message)
    {
        // Implementação específica para WhatsApp
    }
}
```

3. **Registrar o serviço**:
```csharp
services.AddScoped<INotificationService, WhatsAppNotificationService>();
```

### Adicionando Novos Cenários de Notificação

1. **Criar método no ReservaNotificationService**:
```csharp
public async Task NotificarLembreteDevolucaoAsync(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico)
{
    // Implementação do novo cenário
}
```

2. **Integrar no ReservaService**:
```csharp
// No método apropriado
await _notificationService.NotificarLembreteDevolucaoAsync(reservaDto, usuario, periferico);
```

## Boas Práticas

### 1. Tratamento de Erros

- As notificações são enviadas em blocos try-catch
- Falhas na notificação não interrompem a operação principal
- Todos os erros são logados para monitoramento

### 2. Logging

- Todas as operações são logadas com diferentes níveis
- Erros incluem contexto detalhado para debugging
- Sucessos são logados para auditoria

### 3. Configuração

- Todas as configurações são externalizadas no appsettings.json
- Uso de IOptions<T> para injeção de dependência
- Configurações sensíveis devem ser protegidas (User Secrets, Azure Key Vault, etc.)

### 4. Performance

- Operações de notificação são assíncronas
- Suporte para envio em lote de múltiplas mensagens
- Timeout configurável para operações de rede

## Segurança

### 1. Validação de Entrada

- Todas as mensagens são validadas antes do envio
- Sanitização de conteúdo HTML quando aplicável
- Validação de endereços de email

### 2. Configuração Segura

- Senhas e credenciais não são hardcoded
- Uso de configurações seguras para produção
- Suporte para autenticação OAuth2 quando disponível

## Monitoramento

### 1. Métricas

- Contagem de notificações enviadas
- Taxa de sucesso/falha por tipo
- Tempo de resposta dos serviços

### 2. Alertas

- Falhas consecutivas de notificação
- Tempo de resposta acima do esperado
- Quota de envio excedida

## Exemplos de Uso

### 1. Notificação de Solicitação de Reserva

```csharp
await _notificationService.NotificarSolicitacaoReservaAsync(
    reservaDto, 
    usuarioDto, 
    perifericoDto, 
    administradorEquipeDto);
```

### 2. Notificação de Aprovação

```csharp
await _notificationService.NotificarAprovacaoReservaAsync(
    reservaDto,
    usuarioDto,
    perifericoDto,
    "Aprovada",
    null);
```

### 3. Notificação de Rejeição

```csharp
await _notificationService.NotificarAprovacaoReservaAsync(
    reservaDto,
    usuarioDto,
    perifericoDto,
    "Rejeitada",
    "Periférico indisponível no horário solicitado");
```

## Troubleshooting

### Problemas Comuns

1. **Email não enviado**:
   - Verificar configurações SMTP
   - Confirmar credenciais
   - Verificar logs de erro

2. **Notificação não recebida**:
   - Verificar endereço de destino
   - Confirmar se o serviço está registrado
   - Verificar logs de sucesso

3. **Erro de configuração**:
   - Validar appsettings.json
   - Confirmar registro de serviços
   - Verificar injeção de dependência

## Futuras Melhorias

1. **Templates de Email**: Sistema de templates para personalização
2. **Agendamento**: Envio de notificações agendadas
3. **Retry Policy**: Política de retry para falhas temporárias
4. **Webhooks**: Integração com sistemas externos
5. **Métricas Avançadas**: Dashboard de monitoramento
6. **Multi-tenancy**: Suporte para múltiplos provedores


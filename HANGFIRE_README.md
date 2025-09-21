# Hangfire - Sistema de Jobs para Notificações por Email

## Visão Geral

O sistema foi configurado para usar o Hangfire para gerenciar o envio de notificações por email de forma assíncrona, com retry automático em caso de falha.

## Configuração Implementada

### 1. Pacotes Instalados
- `Hangfire.Core` (v1.8.11)
- `Hangfire.AspNetCore` (v1.8.11) 
- `Hangfire.PostgreSql` (v1.20.5)

### 2. Serviços Criados

#### EmailJobService
- **Localização**: `src/ReservaPeriferico.Application/Services/EmailJobService.cs`
- **Funcionalidades**:
  - Enfileiramento de emails individuais e em lote
  - Execução assíncrona de jobs de envio
  - Retry automático (3 tentativas com delays: 30s, 60s, 120s)
  - Logging detalhado de cada tentativa

#### ReservaNotificationService (Atualizado)
- **Modificações**:
  - Agora usa `IEmailJobService` em vez de envio direto
  - Emails são enfileirados para processamento assíncrono
  - Logs indicam que emails foram "enfileirados" em vez de "enviados"

### 3. Configuração do Hangfire

#### Storage
- **Banco**: PostgreSQL (mesmo banco da aplicação)
- **Tabelas**: Criadas automaticamente pelo Hangfire
- **Configurações**:
  - Queue Poll Interval: 15 segundos
  - Invisibility Timeout: 30 minutos
  - Distributed Lock Timeout: 10 minutos

#### Dashboard
- **URL**: `/hangfire` (apenas em desenvolvimento)
- **Acesso**: Permitido apenas em ambiente de desenvolvimento
- **Funcionalidades**:
  - Monitoramento de jobs em tempo real
  - Histórico de execuções
  - Controle manual de jobs (retry, delete, etc.)

## Como Funciona

### 1. Fluxo de Notificação

```
1. Usuário solicita/administrador aprova reserva
2. ReservaNotificationService cria mensagens de email
3. EmailJobService enfileira os emails via Hangfire
4. Hangfire processa os jobs em background
5. Em caso de falha, Hangfire tenta novamente automaticamente
```

### 2. Retry Policy

- **Tentativas**: 3 tentativas automáticas
- **Delays**: 30s → 60s → 120s
- **Falha Final**: Job marcado como "Failed" após 3 tentativas

### 3. Monitoramento

#### Dashboard do Hangfire
Acesse `https://localhost:5001/hangfire` em desenvolvimento para:
- Ver jobs pendentes, processando e concluídos
- Verificar logs de execução
- Reexecutar jobs falhados manualmente
- Monitorar performance e estatísticas

#### Logs da Aplicação
- Jobs enfileirados: `"Job de email enfileirado com ID: {JobId}"`
- Jobs executando: `"Iniciando envio de email para {Email}"`
- Jobs sucesso: `"Email enviado com sucesso para {Email}"`
- Jobs falha: `"Erro no job de envio de email para {Email}"`

## Vantagens da Implementação

### 1. Confiabilidade
- **Retry Automático**: Falhas temporárias são tratadas automaticamente
- **Persistência**: Jobs são salvos no banco, sobrevivem a reinicializações
- **Isolamento**: Falhas em um email não afetam outros

### 2. Performance
- **Assíncrono**: Emails não bloqueiam a interface do usuário
- **Background Processing**: Processamento em paralelo
- **Escalabilidade**: Múltiplos workers podem processar jobs

### 3. Monitoramento
- **Dashboard Visual**: Interface web para monitoramento
- **Logs Detalhados**: Rastreamento completo de cada job
- **Métricas**: Estatísticas de performance e falhas

## Configurações Avançadas

### 1. Workers Personalizados
```csharp
// No Program.cs, você pode configurar múltiplos workers:
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = 4; // 4 workers paralelos
    options.Queues = new[] { "email", "sms", "default" };
});
```

### 2. Jobs Agendados
```csharp
// Para emails agendados (ex: lembretes):
BackgroundJob.Schedule(() => SendReminderEmail(userId), TimeSpan.FromHours(24));
```

### 3. Jobs Recorrentes
```csharp
// Para relatórios periódicos:
RecurringJob.AddOrUpdate("weekly-report", () => SendWeeklyReport(), Cron.Weekly);
```

## Troubleshooting

### 1. Jobs Não Processando
- Verificar se o Hangfire Server está rodando
- Verificar conexão com o banco de dados
- Verificar logs do Hangfire no console

### 2. Emails Não Enviando
- Verificar configurações de email no `appsettings.json`
- Verificar logs do `EmailJobService`
- Verificar se o serviço de email está configurado corretamente

### 3. Dashboard Não Acessível
- Verificar se está em ambiente de desenvolvimento
- Verificar se a URL `/hangfire` está correta
- Verificar configurações de autorização

## Como Testar a Implementação

### 1. Executar a Aplicação
```bash
cd src/ReservaPeriferico.Web
dotnet run
```

### 2. Acessar o Dashboard do Hangfire
- URL: `https://localhost:5001/hangfire`
- Monitorar jobs em tempo real
- Verificar histórico de execuções

### 3. Testar Jobs de Email
Quando uma reserva for solicitada ou aprovada, os emails serão automaticamente enfileirados via Hangfire.

### 4. Endpoints de Teste
Para testar o Hangfire diretamente:

#### Job Imediato
```
GET https://localhost:5001/test/hangfire/immediate
```
Resposta: `{"message": "Job de teste enfileirado", "jobId": "12345"}`

#### Job Agendado
```
GET https://localhost:5001/test/hangfire/delayed/30
```
Resposta: `{"message": "Job de teste agendado para 30 segundos", "jobId": "12346"}`

### 5. Verificar Logs
Os logs mostrarão:
- Jobs enfileirados: `"Job de teste enfileirado com ID: {JobId}"`
- Jobs executando: `"=== JOB DE TESTE EXECUTADO COM SUCESSO ==="`
- Jobs finalizados: `"=== JOB DE TESTE FINALIZADO ==="`

## Próximos Passos

1. **Produção**: Configurar autorização adequada para o dashboard
2. **Métricas**: Implementar métricas de performance
3. **Alertas**: Configurar alertas para falhas críticas
4. **Escalabilidade**: Considerar múltiplos workers em produção

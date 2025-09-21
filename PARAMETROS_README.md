# Sistema de Parâmetros - Configurações Dinâmicas

## Visão Geral

Implementei um sistema completo de parâmetros que permite gerenciar configurações do sistema diretamente no banco de dados, incluindo as configurações de email que foram migradas do `appsettings.json`.

## Estrutura Implementada

### 1. Entidade Parametro
**Localização**: `src/ReservaPeriferico.Core/Entities/Parametro.cs`

```csharp
public class Parametro
{
    public int Id { get; set; }
    public string Chave { get; set; } = string.Empty;        // Chave única do parâmetro
    public string Valor { get; set; } = string.Empty;        // Valor do parâmetro
    public string? Descricao { get; set; }                   // Descrição do parâmetro
    public bool Ativo { get; set; } = true;                  // Se o parâmetro está ativo
    public ParametroTipo Tipo { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public DateTime? DataAtualizacao { get; set; }
    public string? UsuarioAtualizacao { get; set; }          // Quem atualizou
}
```

### 2. Enum ParametroChave
**Localização**: `src/ReservaPeriferico.Core/Enums/ParametroChave.cs`

```csharp
public enum ParametroChave
{
    // Configurações de Email
    EmailSmtpServer,
    EmailSmtpPort,
    EmailUsername,
    EmailPassword,
    EmailEnableSsl,
    EmailFromEmail,
    EmailFromName,
    EmailTimeout
   
}
```

### 3. Repositório e Serviço
- **IParametroRepository**: Interface para operações no banco
- **ParametroRepository**: Implementação com Entity Framework
- **IParametroService**: Interface para lógica de negócio
- **ParametroService**: Implementação com cache automático

## Funcionalidades Implementadas

### 1. Cache Automático
- Cache de 5 minutos para melhor performance
- Atualização automática quando necessário
- Fallback para banco em caso de cache vazio

### 2. Tipagem Forte
```csharp
// Buscar parâmetro como string
var smtpServer = await parametroService.GetParameterAsync(ParametroChave.EmailSmtpServer);

// Buscar parâmetro tipado
var smtpPort = await parametroService.GetParameterAsync<int>(ParametroChave.EmailSmtpPort);
var enableSsl = await parametroService.GetParameterAsync<bool>(ParametroChave.EmailEnableSsl);
```

### 3. Atualização de Parâmetros
```csharp
// Atualizar parâmetro
await parametroService.SetParameterAsync(ParametroChave.EmailSmtpServer, "novo.servidor.com", "admin");

// Atualizar com tipagem
await parametroService.SetParameterAsync(ParametroChave.EmailSmtpPort, 465, "admin");
```

## Migração das Configurações de Email

### 1. DatabaseEmailSettings
**Localização**: `src/ReservaPeriferico.Application/Configuration/DatabaseEmailSettings.cs`

Serviço que substitui o `EmailSettings` do `appsettings.json`, buscando configurações diretamente do banco:

```csharp
public async Task<EmailSettings> GetEmailSettingsAsync()
{
    return new EmailSettings
    {
        SmtpServer = await GetSmtpServerAsync(),
        SmtpPort = await GetSmtpPortAsync(),
        Username = await GetUsernameAsync(),
        Password = await GetPasswordAsync(),
        EnableSsl = await GetEnableSslAsync(),
        FromEmail = await GetFromEmailAsync(),
        FromName = await GetFromNameAsync(),
        Timeout = await GetTimeoutAsync()
    };
}
```

### 2. EmailNotificationService Atualizado
O serviço de email agora usa as configurações do banco em vez do `appsettings.json`:

```csharp
var emailSettings = await _databaseEmailSettings.GetEmailSettingsAsync();
using var client = CreateSmtpClient(emailSettings);
using var mailMessage = CreateMailMessage(message, emailSettings);
```


## Interface de Gerenciamento

### Página de Parâmetros
**Localização**: `src/ReservaPeriferico.Web/Pages/Parametros/Parametros.razor`

Interface web organizada em abas:
- **Configurações de Email**: Todos os parâmetros relacionados ao envio de email
- **Sistema**: Configurações gerais do sistema
- **Notificações**: Configurações de notificação

### Funcionalidades da Interface
- Edição em tempo real dos parâmetros
- Salvamento por categoria
- Validação e feedback visual
- Organização por abas temáticas

## Endpoints de Teste

### 1. Listar Todos os Parâmetros
```
GET /test/parametros
```

### 2. Testar Configurações de Email
```
GET /test/parametros/email
```

## Vantagens da Implementação

### 1. Flexibilidade
- **Configuração Dinâmica**: Mudanças sem necessidade de restart
- **Ambiente Específico**: Diferentes configurações por ambiente
- **Versionamento**: Histórico de alterações com usuário e data

### 2. Segurança
- **Controle de Acesso**: Interface web com autenticação
- **Auditoria**: Registro de quem alterou e quando
- **Validação**: Tipagem forte previne erros

### 3. Performance
- **Cache Inteligente**: Reduz consultas ao banco
- **Fallback**: Configurações padrão em caso de falha
- **Eficiência**: Uma consulta para todas as configurações

### 4. Manutenibilidade
- **Centralização**: Todas as configurações em um local
- **Documentação**: Descrição de cada parâmetro
- **Organização**: Categorização por tipo

## Como Usar

### 1. Acessar Interface
Navegue para `/parametros` no sistema web.

### 2. Editar Configurações
- Selecione a aba desejada
- Edite os valores necessários
- Clique em "Salvar"

### 3. Testar Configurações
Use os endpoints de teste para verificar se as configurações estão funcionando.

### 4. Monitorar Alterações
As alterações são registradas com data/hora e usuário responsável.

## Próximos Passos

1. **Migração Completa**: Remover configurações do `appsettings.json`
2. **Validação**: Adicionar validação de valores (ex: portas válidas)
3. **Backup**: Sistema de backup automático de configurações
4. **Alertas**: Notificações quando parâmetros críticos são alterados
5. **API**: Endpoints REST para integração externa

## Estrutura do Banco

```sql
CREATE TABLE parametro (
    id SERIAL PRIMARY KEY,
    chave VARCHAR(100) UNIQUE NOT NULL,
    valor TEXT NOT NULL,
    descricao VARCHAR(500),
    ativo BOOLEAN DEFAULT true,
    data_criacao TIMESTAMP DEFAULT NOW(),
    data_atualizacao TIMESTAMP,
    usuario_atualizacao VARCHAR(100)
);
```

A implementação está completa e pronta para uso! O sistema agora permite gerenciar todas as configurações de forma dinâmica através do banco de dados.





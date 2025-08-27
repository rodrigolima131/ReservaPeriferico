# Configuração do Google OAuth para Login

## Passos para Configurar

### 1. Criar Projeto no Google Cloud Console

1. Acesse [Google Cloud Console](https://console.cloud.google.com/)
2. Crie um novo projeto ou selecione um existente
3. Ative a API "Google+ API" ou "Google Identity"

### 2. Configurar Credenciais OAuth 2.0

1. No menu lateral, vá para "APIs & Services" > "Credentials"
2. Clique em "Create Credentials" > "OAuth 2.0 Client IDs"
3. Configure o tipo de aplicação:
   - **Application type**: Web application
   - **Name**: ReservaPeriferico (ou nome de sua preferência)
   - **Authorized redirect URIs**: 
     - `https://localhost:7000/signin-oidc` (desenvolvimento)
     - `https://seudominio.com/signin-oidc` (produção)

### 3. Configurar o User Secrets (RECOMENDADO)

**NÃO** coloque credenciais reais no `appsettings.json`! Use User Secrets:

```bash
cd src/ReservaPeriferico.Web

# Configurar o Client ID
dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_CLIENT_ID_REAL.apps.googleusercontent.com"

# Configurar o Client Secret
dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_CLIENT_SECRET_REAL"
```

### 4. Alternativa: appsettings.json (NÃO RECOMENDADO)

Se preferir usar arquivo de configuração (não recomendado para segurança):

```json
{
  "GoogleOAuth": {
    "ClientId": "SEU_CLIENT_ID_AQUI.apps.googleusercontent.com",
    "ClientSecret": "SEU_CLIENT_SECRET_AQUI"
  }
}
```

> **🚨 ATENÇÃO**: Credenciais em arquivos de configuração podem ser commitadas acidentalmente!

### 5. URLs de Redirecionamento Suportadas

O sistema suporta as seguintes URLs de redirecionamento:
- `/signin-google` (legado)
- `/signin-oidc` (padrão ASP.NET Core)

### 6. Testar a Configuração

1. Execute a aplicação
2. Acesse a página de login
3. Clique em "Entrar com Google"
4. Você será redirecionado para o Google para autenticação
5. Após o login, será redirecionado de volta para o dashboard

## Solução de Problemas

### Erro: "Invalid redirect_uri"
- Verifique se a URI de redirecionamento no Google Cloud Console está correta
- Certifique-se de incluir o protocolo (http/https) e a porta

### Erro: "Client ID not found"
- Verifique se o ClientId está correto no User Secrets ou appsettings.json
- Certifique-se de que o projeto está ativo no Google Cloud Console

### Erro: "Client secret mismatch"
- Verifique se o ClientSecret está correto no User Secrets ou appsettings.json
- Regenerar o ClientSecret se necessário

## Segurança

- **NUNCA** commite credenciais reais no controle de versão
- **USE User Secrets** para desenvolvimento local (RECOMENDADO)
- Use variáveis de ambiente ou Azure Key Vault para produção
- Configure HTTPS em produção
- **NUNCA** coloque credenciais reais em arquivos markdown ou de documentação

## Estrutura de URLs

- **Login**: `/` ou `/login`
- **Callback Google**: `/signin-oidc`
- **Dashboard**: `/dashboard`
- **Logout**: `/logout` 
# Configuração do appsettings.Example.json

## Sobre este arquivo
Este arquivo serve como **exemplo** de configuração para o projeto. Ele contém apenas as configurações básicas que podem ser compartilhadas no repositório.

## Configurações incluídas
- **ConnectionStrings**: Configuração do banco de dados PostgreSQL
- **Logging**: Configurações de log da aplicação
- **AllowedHosts**: Hosts permitidos para a aplicação

## Configurações NÃO incluídas (por segurança)
- **Google OAuth**: Credenciais de autenticação
- **Senhas**: Qualquer tipo de senha ou chave secreta
- **Tokens**: Tokens de API ou autenticação

## Como configurar o Google OAuth
Para configurar o Google OAuth, use o **User Secrets** em vez de modificar este arquivo:

```bash
cd src/ReservaPeriferico.Web
dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_GOOGLE_CLIENT_ID"
dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_GOOGLE_CLIENT_SECRET"
```

## Para usar em desenvolvimento
1. Copie este arquivo para `appsettings.Development.json`
2. Ajuste a connection string do banco de dados conforme necessário
3. Configure o Google OAuth usando User Secrets
4. Execute a aplicação

## Para produção
- Use variáveis de ambiente
- Use Azure Key Vault ou similar
- **NUNCA** coloque credenciais reais em arquivos de configuração

## Veja também
- [USER_SECRETS_SETUP.md](USER_SECRETS_SETUP.md) - Configuração detalhada do Google OAuth
- [README.md](../../README.md) - Documentação principal do projeto 
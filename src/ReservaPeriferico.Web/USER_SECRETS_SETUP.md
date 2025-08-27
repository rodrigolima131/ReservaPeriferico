# Configuração do User Secrets para Google OAuth

## O que é User Secrets?
O User Secrets é uma funcionalidade do .NET que permite armazenar informações sensíveis (como chaves de API, senhas, etc.) fora do código fonte, de forma segura e local.

## Como configurar:

### 1. Verificar se o User Secrets está habilitado
O projeto já tem o `UserSecretsId` configurado no arquivo `.csproj`:
```xml
<UserSecretsId>8503414c-2872-44f2-90c9-bda21182fda9</UserSecretsId>
```

### 2. Configurar as credenciais do Google OAuth
Execute os seguintes comandos no terminal, na pasta do projeto (`src/ReservaPeriferico.Web`):

```bash
# Configurar o Client ID do Google
dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_GOOGLE_CLIENT_ID_AQUI"

# Configurar o Client Secret do Google
dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_GOOGLE_CLIENT_SECRET_AQUI"
```

### 3. Verificar se as configurações foram salvas
```bash
# Listar todos os secrets configurados
dotnet user-secrets list
```

### 4. Estrutura do User Secrets
O arquivo será criado automaticamente em:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\8503414c-2872-44f2-90c9-bda21182fda9\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/8503414c-2872-44f2-90c9-bda21182fda9/secrets.json`

### 5. Conteúdo esperado do secrets.json
```json
{
  "GoogleOAuth:ClientId": "seu_client_id_aqui",
  "GoogleOAuth:ClientSecret": "seu_client_secret_aqui"
}
```

## Vantagens do User Secrets:
- ✅ **Segurança**: Credenciais não ficam no código fonte
- ✅ **Desenvolvimento**: Cada desenvolvedor pode ter suas próprias credenciais
- ✅ **Git**: Arquivo não é commitado no repositório
- ✅ **Flexibilidade**: Fácil de alterar sem recompilar

## Importante:
- **NUNCA** commite o arquivo `secrets.json` no Git
- O arquivo já está no `.gitignore` por padrão
- Use apenas para desenvolvimento local
- Para produção, use variáveis de ambiente ou Azure Key Vault

## Troubleshooting:
Se receber erro "Google OAuth credentials not configured in User Secrets":
1. Verifique se está na pasta correta do projeto
2. Execute `dotnet user-secrets list` para verificar se as configurações foram salvas
3. Reinicie a aplicação após configurar os secrets 
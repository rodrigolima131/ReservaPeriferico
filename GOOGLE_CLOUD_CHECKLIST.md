# Checklist de Verificação - Google Cloud Console

## 🔍 Verificações Obrigatórias

### 1. **Projeto e APIs**
- [ ] Projeto criado no Google Cloud Console
- [ ] API "Google+ API" ou "Google Identity" **ATIVADA**
- [ ] API "Google OAuth2 API" **ATIVADA**

### 2. **Credenciais OAuth 2.0**
- [ ] Tipo: **"Aplicativo da Web"** (NÃO "App para computador")
- [ ] Nome: ReservaPeriferico (ou similar)
- [ ] **URIs de redirecionamento autorizados** incluem:
  - `http://localhost:5001/signin-oidc` ✅
  - `https://localhost:5001/signin-oidc` ✅ (se usar HTTPS)

### 3. **URIs de Redirecionamento - IMPORTANTE**
```
http://localhost:5001/signin-oidc
```
**NÃO** incluir:
- ❌ `/signin-google`
- ❌ `/callback`
- ❌ `/auth/google`

### 4. **Verificar se as credenciais estão corretas**
- [ ] Client ID: `SEU_CLIENT_ID_AQUI.apps.googleusercontent.com`
- [ ] Client Secret: `SEU_CLIENT_SECRET_AQUI`

> **⚠️ IMPORTANTE**: Use User Secrets para as credenciais reais:
> ```bash
> dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_CLIENT_ID_REAL"
> dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_CLIENT_SECRET_REAL"
> ```

## 🚨 Problemas Comuns

### **"oauth state was missing or invalid"**
- **Causa**: URI de redirecionamento incorreta no Google Cloud Console
- **Solução**: Verificar se `http://localhost:5001/signin-oidc` está configurado

### **"redirect_uri_mismatch"**
- **Causa**: URI de redirecionamento não corresponde
- **Solução**: Copiar exatamente a URI configurada

### **"invalid_client"**
- **Causa**: Client ID ou Secret incorretos
- **Solução**: Verificar se as credenciais estão corretas no appsettings.json

## 📋 Passos para Testar

1. **Verificar configuração** no Google Cloud Console
2. **Executar aplicação**: `dotnet run`
3. **Clicar em "Entrar com Google"**
4. **Verificar console** para logs de debug
5. **Verificar se redireciona** para Google

## 🔧 Se ainda não funcionar

1. **Regenerar Client Secret** no Google Cloud Console
2. **Verificar se não há espaços** nas URIs de redirecionamento
3. **Limpar cache** do navegador
4. **Verificar se não há firewall** bloqueando

## 📞 URLs de Verificação

- **Google Cloud Console**: https://console.cloud.google.com/
- **APIs & Services > Credentials**: https://console.cloud.google.com/apis/credentials
- **APIs & Services > Library**: https://console.cloud.google.com/apis/library 
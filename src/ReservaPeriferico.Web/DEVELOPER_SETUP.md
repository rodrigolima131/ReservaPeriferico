# 🚀 Roteiro de Configuração para Desenvolvedores

## 📋 **Pré-requisitos**
- .NET 9 SDK instalado
- PostgreSQL rodando
- Git instalado

## 🔧 **Setup Inicial**

### 1. **Clone e Dependências**
```bash
# Clonar o projeto
git clone [URL_DO_REPOSITORIO]
cd ReservaPeriferico

# Restaurar dependências
dotnet restore
```

### 2. **Configurar Banco de Dados**
```bash
# Ajustar connection string em appsettings.json (se necessário)
# Padrão: Host=localhost;Database=ReservaPeriferico;Username=postgres;Password=postgres

# Aplicar migrations
dotnet ef database update --startup-project src/ReservaPeriferico.Web
```

### 3. **Configurar Google OAuth (OBRIGATÓRIO)**
```bash
# Navegar para o projeto Web
cd src/ReservaPeriferico.Web

# ⚠️ IMPORTANTE: NÃO precisa rodar "dotnet user-secrets init"
# O projeto já está configurado com UserSecretsId

# Configurar credenciais (receber por canal seguro)
dotnet user-secrets set "GoogleOAuth:ClientId" "CLIENT_ID_RECEBIDO"
dotnet user-secrets set "GoogleOAuth:ClientSecret" "CLIENT_SECRET_RECEBIDO"

# Verificar se foi salvo
dotnet user-secrets list
```

### 4. **Testar a Aplicação**
```bash
# Executar a aplicação
dotnet run --launch-profile https

# Acessar: https://localhost:5001
```

## 🚨 **IMPORTANTE - Segurança**

### **O que NÃO fazer:**
- ❌ **NUNCA** commite credenciais reais
- ❌ **NUNCA** coloque credenciais em arquivos markdown
- ❌ **NUNCA** coloque credenciais em appsettings.json
- ❌ **NUNCA** rode `dotnet user-secrets init` (já está configurado)

### **O que SEMPRE fazer:**
- ✅ **SEMPRE** use User Secrets para credenciais
- ✅ **SEMPRE** receba credenciais por canal seguro
- ✅ **SEMPRE** verifique se as credenciais foram salvas

## 🔍 **Troubleshooting**

### **Erro: "Google OAuth credentials not configured"**
```bash
# Verificar se está na pasta correta
pwd  # deve mostrar: .../ReservaPeriferico.Web

# Verificar se as credenciais foram salvas
dotnet user-secrets list

# Se estiver vazio, configurar novamente
dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_CLIENT_ID"
dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_CLIENT_SECRET"
```

### **Erro: "Invalid redirect_uri"**
- Verificar se no Google Cloud Console está configurado: `https://localhost:5001/signin-google`

### **Erro: "Client ID not found"**
- Verificar se o Client ID está correto
- Verificar se o projeto está ativo no Google Cloud Console

## 📱 **Canal Seguro para Credenciais**

### **Opções recomendadas:**
1. **WhatsApp/Telegram** (criptografado)
2. **Email corporativo** (interno)
3. **Slack/Discord** (canal privado)
4. **1Password/LastPass** (gerenciador de senhas)
5. **Google Drive** (compartilhamento restrito)

### **NUNCA enviar por:**
- ❌ Email público
- ❌ Chat público
- ❌ Repositório Git
- ❌ Documentos públicos

## 🎯 **Checklist Final**

Antes de testar, verificar:
- [ ] Projeto clonado ✅
- [ ] Dependências restauradas ✅
- [ ] Banco configurado ✅
- [ ] Migrations aplicadas ✅
- [ ] User Secrets configurado ✅
- [ ] Credenciais recebidas por canal seguro ✅
- [ ] Aplicação executando ✅

## 📞 **Suporte**

Se der problema:
1. Verificar console da aplicação
2. Verificar se User Secrets está configurado
3. Verificar se credenciais são válidas
4. Verificar se banco está rodando
5. Pedir ajuda no grupo/chat da equipe

---

## 🆘 **Dúvidas Frequentes**

### **"Preciso rodar dotnet user-secrets init?"**
**NÃO!** O projeto já está configurado. Apenas configure as credenciais.

### **"Onde fica o arquivo secrets.json?"**
Automaticamente em:
- **Windows**: `%APPDATA%\Microsoft\UserSecrets\8503414c-2872-44f2-90c9-bda21182fda9\secrets.json`
- **macOS/Linux**: `~/.microsoft/usersecrets/8503414c-2872-44f2-90c9-bda21182fda9/secrets.json`

### **"Posso usar appsettings.json em vez de User Secrets?"**
**NÃO RECOMENDADO** por segurança. Use sempre User Secrets.

---

**Agora seus amigos têm um roteiro completo e seguro! 🚀** 
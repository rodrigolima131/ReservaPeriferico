# 🚨 Diretrizes de Segurança - ReservaPeriferico

## ⚠️ REGRAS CRÍTICAS DE SEGURANÇA

### 1. **NUNCA Commite Credenciais Reais**
- ❌ **NUNCA** coloque Client IDs reais em arquivos markdown
- ❌ **NUNCA** coloque Client Secrets reais em arquivos markdown  
- ❌ **NUNCA** coloque credenciais em arquivos de configuração commitados
- ❌ **NUNCA** coloque senhas ou tokens em código fonte

### 2. **Arquivos que NÃO devem conter credenciais reais:**
- `GOOGLE_CLOUD_CHECKLIST.md`
- `GOOGLE_OAUTH_SETUP.md`
- `README.md`
- `appsettings.json`
- `appsettings.Development.json`
- Qualquer arquivo markdown ou de documentação

### 3. **O que fazer se encontrar credenciais reais:**
1. **IMMEDIATAMENTE** remova as credenciais do arquivo
2. **Regenere** as credenciais no Google Cloud Console
3. **Revogue** as credenciais antigas
4. **Notifique** a equipe de segurança
5. **Verifique** o histórico do Git para remover commits anteriores

## ✅ Práticas Seguras

### 1. **Use User Secrets (RECOMENDADO)**
```bash
cd src/ReservaPeriferico.Web
dotnet user-secrets set "GoogleOAuth:ClientId" "SEU_CLIENT_ID_REAL"
dotnet user-secrets set "GoogleOAuth:ClientSecret" "SEU_CLIENT_SECRET_REAL"
```

### 2. **Use Placeholders em Documentação**
```markdown
- Client ID: `SEU_CLIENT_ID_AQUI.apps.googleusercontent.com`
- Client Secret: `SEU_CLIENT_SECRET_AQUI`
```

### 3. **Arquivos de Exemplo**
- Use `secrets.json.example` com valores fictícios
- Use `appsettings.Example.json` sem credenciais reais

## 🔍 Verificação de Segurança

### Antes de cada commit, verifique:
- [ ] Nenhum arquivo markdown contém credenciais reais
- [ ] Nenhum arquivo de configuração contém credenciais reais
- [ ] Nenhum arquivo de exemplo contém credenciais reais
- [ ] User Secrets está configurado corretamente

### Comandos para verificar:
```bash
# Verificar se há credenciais reais nos arquivos
grep -r "apps.googleusercontent.com" . --exclude-dir=bin --exclude-dir=obj
grep -r "GOCSPX-" . --exclude-dir=bin --exclude-dir=obj

# Verificar User Secrets
dotnet user-secrets list
```

## 🚨 Emergência - Credenciais Expostas

### Se credenciais foram commitadas:
1. **IMMEDIATAMENTE** regenere no Google Cloud Console
2. **Force push** para sobrescrever o histórico (se necessário)
3. **Notifique** todos os desenvolvedores
4. **Revogue** credenciais antigas
5. **Monitore** logs de acesso

## 📞 Contatos de Segurança
- Equipe de Desenvolvimento
- Administrador do Sistema
- Google Cloud Console Support

---

**Lembre-se: Segurança é responsabilidade de todos!** 🛡️ 
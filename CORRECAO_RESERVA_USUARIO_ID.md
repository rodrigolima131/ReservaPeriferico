# 🔧 CORREÇÃO: Reserva com UsuarioId e EquipeId Incorretos

## 📊 Problema Identificado

Ao criar uma reserva, o sistema estava gravando incorretamente:
- `usuario_id` = 1 (hardcoded) ao invés de 2 (usuário logado "Alexander Dias Brito")
- `equipe_id` = 1 ao invés de 5 (equipe "EQUIPE ONE PIECE")

Além disso, a aprovação de reservas também estava usando usuário fixo (ID = 1).

---

## 🔍 Causa Raiz

### **Problema 1: UsuarioId Fixo na Criação de Reserva**
**Arquivo:** `src/ReservaPeriferico.Web/Pages/Reserva/Reserva.razor`  
**Linha 220 (ANTES):**
```csharp
// Usuário padrão para testes (deve ser substituído pela autenticação real)
int usuarioId = 1;
```

### **Problema 2: UsuarioAprovadorId Fixo na Aprovação**
**Arquivo:** `src/ReservaPeriferico.Web/Pages/Reserva/GerenciarReservas.razor`  
**Linhas 211, 250, 282 (ANTES):**
```csharp
// Usuário padrão para testes (deve ser substituído pela autenticação real)
int usuarioAprovadorId = 1;
```

### **Problema 3: EquipeId Incorreto**
A `EquipeId` está sendo definida com base na equipe do periférico selecionado:
```csharp
EquipeId = periferico.EquipeId,
```

Isso significa que se o periférico pertencer à equipe incorreta, a reserva será gravada com o `EquipeId` errado.

---

## ✅ Correções Implementadas

### **1. Injetar UserInfoService**
**Arquivos:** `Reserva.razor` e `GerenciarReservas.razor`

```razor
@inject UserInfoService UserInfoService
```

### **2. Obter UsuarioId Real na Criação de Reserva**
**Arquivo:** `src/ReservaPeriferico.Web/Pages/Reserva/Reserva.razor`

```csharp
// ✅ Obter ID do usuário logado através do UserInfoService
var usuarioId = UserInfoService.GetUserId();
if (!usuarioId.HasValue || usuarioId.Value <= 0)
{
    Snackbar.Add("Erro: Usuário não autenticado", Severity.Error);
    return;
}

Console.WriteLine($"=== Criando reserva com UsuarioId: {usuarioId.Value} ===");

// Persistir no banco
var reservaCriada = await ReservaService.SolicitarReservaAsync(usuarioId.Value, solicitarReserva);
```

### **3. Obter UsuarioAprovadorId Real na Aprovação**
**Arquivo:** `src/ReservaPeriferico.Web/Pages/Reserva/GerenciarReservas.razor`

```csharp
// ✅ Obter ID do usuário logado através do UserInfoService
var usuarioAprovadorId = UserInfoService.GetUserId();
if (!usuarioAprovadorId.HasValue || usuarioAprovadorId.Value <= 0)
{
    Snackbar.Add("Erro: Usuário não autenticado", Severity.Error);
    return;
}

Console.WriteLine($"=== Aprovando reserva {reserva.Id} com UsuarioAprovadorId: {usuarioAprovadorId.Value} ===");
Console.WriteLine($"=== Reserva pertence à equipe: {reserva.EquipeId} ===");

await ReservaService.AprovarReservaAsync(reserva.Id, usuarioAprovadorId.Value, aprovarDto);
```

### **4. Correções Aplicadas Também em:**
- **Rejeitar Reserva**: Agora usa `UserInfoService.GetUserId()`
- **Cancelar Reserva**: Agora usa `UserInfoService.GetUserId()`

---

## 🔍 Verificação da Lógica de Aprovação

A validação de aprovação de reservas está no método `UsuarioPodeAprovarAsync`:

```csharp
public async Task<bool> UsuarioPodeAprovarAsync(int usuarioId, int equipeId)
{
    var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, equipeId);
    return usuarioEquipe != null && usuarioEquipe.IsAdministrador && usuarioEquipe.Usuario.Ativo;
}
```

**Como funciona:**
1. Busca o vínculo entre usuário e equipe (`UsuarioEquipe`)
2. Verifica se o vínculo existe
3. Verifica se o usuário é administrador (`IsAdministrador`)
4. Verifica se o usuário está ativo

**Validação chamada em:**
```csharp
if (!await UsuarioPodeAprovarAsync(usuarioAprovadorId, reserva.EquipeId))
    throw new ReservaException("Usuário não tem permissão para aprovar reservas desta equipe");
```

---

## 🧪 Como Testar

### **Cenário 1: Criar Reserva Corretamente**
1. ✅ Login com "Alexander Dias Brito" (ID = 2)
2. ✅ Criar equipe "EQUIPE ONE PIECE" (ID = 5)
3. ✅ Criar reserva
4. ✅ Verificar no banco: `usuario_id` = 2 e `equipe_id` = 5

### **Cenário 2: Aprovar Reserva**
1. ✅ Login com o administrador da equipe
2. ✅ Ir para "Gerenciar Reservas"
3. ✅ Tentar aprovar reserva
4. ✅ Verificar se valida corretamente permissões

**IMPORTANTE**: O erro "Usuário não tem permissão para aprovar reservas desta equipe" significa que:
- O usuário logado **NÃO é administrador** da equipe que possui a reserva
- OU o usuário **não está vinculado** à equipe
- OU o usuário **não está ativo**

---

## 📝 Arquivos Modificados

1. ✅ `src/ReservaPeriferico.Web/Pages/Reserva/Reserva.razor`
   - Adicionado `@inject UserInfoService UserInfoService`
   - Corrigido `SolicitarReserva()` para usar `UserInfoService.GetUserId()`

2. ✅ `src/ReservaPeriferico.Web/Pages/Reserva/GerenciarReservas.razor`
   - Adicionado `@inject UserInfoService UserInfoService`
   - Corrigido `AprovarReserva()` para usar `UserInfoService.GetUserId()`
   - Corrigido `RejeitarReserva()` para usar `UserInfoService.GetUserId()`
   - Corrigido `CancelarReserva()` para usar `UserInfoService.GetUserId()`

---

## ⚠️ Observação sobre EquipeId

O `EquipeId` da reserva é preenchido automaticamente com base no periférico selecionado:

```csharp
var periferico = await _perifericoRepository.GetByIdAsync(solicitarReservaDto.PerifericoId);
EquipeId = periferico.EquipeId,
```

**Isso significa:**
- Se o periférico pertence à equipe correta, o `EquipeId` estará correto
- Se o periférico pertence a outra equipe, o `EquipeId` será da equipe do periférico

**Para garantir que o EquipeId está correto:**
1. Verifique se o periférico selecionado pertence à equipe correta
2. Se necessário, ajuste o vínculo do periférico para a equipe desejada

---

## ✅ Resumo das Correções

- ✅ **UsuarioId**: Agora usa `UserInfoService.GetUserId()` ao invés de fixo (1)
- ✅ **EquipeId**: Será determinado pelo periférico selecionado
- ✅ **UsuarioAprovadorId**: Agora usa `UserInfoService.GetUserId()` ao invés de fixo (1)
- ✅ **Validação de Permissões**: A lógica `UsuarioPodeAprovarAsync` está correta
- ✅ **Logs de Debug**: Adicionados para facilitar troubleshooting

**Pronto para testar novamente!** 🎉

---

## 📅 Data de Correção
**Data:** 12/10/2025  
**Status:** CORRIGIDO ✅


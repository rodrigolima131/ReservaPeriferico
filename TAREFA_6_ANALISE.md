# 📝 TAREFA 6 - ANÁLISE DE IMPLEMENTAÇÃO

## ❌ TAREFA 6: Validar exclusão de equipe com reservas ativas

### 📊 Status: **NÃO IMPLEMENTADA** ❌

### 🎯 Objetivo
No `EquipeService.DeleteAsync`, validar se a equipe possui reservas ativas ou pendentes antes de permitir a exclusão.

---

## 🔍 ANÁLISE COMPLETA

### ❌ **DESCOBERTA IMPORTANTE:**
A **Tarefa 6 NÃO está implementada!** A validação de reservas ativas está ausente no método `DeleteAsync`.

### 📋 **IMPLEMENTAÇÃO ATUAL (INCOMPLETA):**

#### **Backend - EquipeService.cs (Validação de Segurança)**

**Validação no DeleteAsync (ATUAL):**
```csharp
public async Task DeleteAsync(int id)
{
    var existingEquipe = await _equipeRepository.GetByIdAsync(id);
    if (existingEquipe == null)
        throw new ArgumentException("Equipe não encontrada");

    // ✅ VALIDAÇÃO DE SEGURANÇA - Obter usuário logado
    var usuarioLogadoId = GetUsuarioLogadoId();
    if (!usuarioLogadoId.HasValue)
    {
        throw new UnauthorizedAccessException("Usuário não autenticado");
    }

    // ✅ VALIDAÇÃO DE SEGURANÇA - Verificar se é o administrador principal
    if (existingEquipe.UsuarioAdministradorId != usuarioLogadoId.Value)
    {
        throw new UnauthorizedAccessException("Apenas o administrador principal pode excluir esta equipe");
    }

    // ❌ FALTA: VALIDAÇÃO DE RESERVAS ATIVAS/PENDENTES
    // ❌ FALTA: Verificar se equipe tem reservas ativas
    // ❌ FALTA: Verificar se equipe tem reservas pendentes
    // ❌ FALTA: Verificar se equipe tem reservas aprovadas

    // Remover todos os membros primeiro
    var membros = await _usuarioEquipeRepository.GetByEquipeIdAsync(id);
    foreach (var membro in membros)
    {
        await _usuarioEquipeRepository.RemoveMembroAsync(id, membro.UsuarioId);
    }

    await _equipeRepository.DeleteAsync(id);
}
```

---

## 🔍 **ANÁLISE DE DEPENDÊNCIAS**

### **✅ RECURSOS DISPONÍVEIS:**

#### **IReservaRepository (Interface):**
```csharp
public interface IReservaRepository : IRepository<Reserva>
{
    Task<IEnumerable<Reserva>> GetByEquipeAsync(int equipeId);
    Task<IEnumerable<Reserva>> GetPendentesAsync(int equipeId);
    Task<IEnumerable<Reserva>> GetAprovadasAsync(int equipeId);
    Task<IEnumerable<Reserva>> GetAtivasAsync();
    // ... outros métodos
}
```

#### **Métodos Disponíveis:**
- ✅ `GetByEquipeAsync(int equipeId)` - Todas as reservas da equipe
- ✅ `GetPendentesAsync(int equipeId)` - Reservas pendentes da equipe
- ✅ `GetAprovadasAsync(int equipeId)` - Reservas aprovadas da equipe
- ✅ `GetAtivasAsync()` - Todas as reservas ativas (sistema)

### **❌ DEPENDÊNCIAS AUSENTES:**

#### **EquipeService não tem acesso ao ReservaRepository:**
```csharp
public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioEquipeRepository _usuarioEquipeRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    // ❌ FALTA: IReservaRepository _reservaRepository;
    
    public EquipeService(
        IEquipeRepository equipeRepository, 
        IUsuarioEquipeRepository usuarioEquipeRepository, 
        IHttpContextAccessor httpContextAccessor
        // ❌ FALTA: IReservaRepository reservaRepository
    )
    {
        _equipeRepository = equipeRepository;
        _usuarioEquipeRepository = usuarioEquipeRepository;
        _httpContextAccessor = httpContextAccessor;
        // ❌ FALTA: _reservaRepository = reservaRepository;
    }
}
```

---

## 🚨 **PROBLEMAS IDENTIFICADOS**

### **1. Validação de Reservas Ausente**
- ❌ Não verifica se equipe tem reservas ativas
- ❌ Não verifica se equipe tem reservas pendentes
- ❌ Não verifica se equipe tem reservas aprovadas
- ❌ Permite exclusão mesmo com reservas ativas

### **2. Dependência Ausente**
- ❌ `IReservaRepository` não está injetado no `EquipeService`
- ❌ Não há acesso aos métodos de verificação de reservas

### **3. Risco de Integridade**
- ❌ Exclusão de equipe com reservas ativas pode quebrar integridade referencial
- ❌ Reservas podem ficar "órfãs" sem equipe associada
- ❌ Dados inconsistentes no sistema

---

## 🎯 **IMPLEMENTAÇÃO NECESSÁRIA**

### **1. Injeção de Dependência**
```csharp
public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioEquipeRepository _usuarioEquipeRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReservaRepository _reservaRepository; // 🆕 ADICIONAR

    public EquipeService(
        IEquipeRepository equipeRepository, 
        IUsuarioEquipeRepository usuarioEquipeRepository, 
        IHttpContextAccessor httpContextAccessor,
        IReservaRepository reservaRepository // 🆕 ADICIONAR
    )
    {
        _equipeRepository = equipeRepository;
        _usuarioEquipeRepository = usuarioEquipeRepository;
        _httpContextAccessor = httpContextAccessor;
        _reservaRepository = reservaRepository; // 🆕 ADICIONAR
    }
}
```

### **2. Validação de Reservas**
```csharp
public async Task DeleteAsync(int id)
{
    var existingEquipe = await _equipeRepository.GetByIdAsync(id);
    if (existingEquipe == null)
        throw new ArgumentException("Equipe não encontrada");

    // ✅ VALIDAÇÃO DE SEGURANÇA - Obter usuário logado
    var usuarioLogadoId = GetUsuarioLogadoId();
    if (!usuarioLogadoId.HasValue)
    {
        throw new UnauthorizedAccessException("Usuário não autenticado");
    }

    // ✅ VALIDAÇÃO DE SEGURANÇA - Verificar se é o administrador principal
    if (existingEquipe.UsuarioAdministradorId != usuarioLogadoId.Value)
    {
        throw new UnauthorizedAccessException("Apenas o administrador principal pode excluir esta equipe");
    }

    // 🆕 VALIDAÇÃO DE RESERVAS - Verificar se equipe tem reservas ativas
    var reservasAtivas = await _reservaRepository.GetByEquipeAsync(id);
    var reservasPendentes = await _reservaRepository.GetPendentesAsync(id);
    var reservasAprovadas = await _reservaRepository.GetAprovadasAsync(id);

    if (reservasAtivas.Any() || reservasPendentes.Any() || reservasAprovadas.Any())
    {
        throw new InvalidOperationException("Não é possível excluir a equipe pois ela possui reservas ativas, pendentes ou aprovadas");
    }

    // Remover todos os membros primeiro
    var membros = await _usuarioEquipeRepository.GetByEquipeIdAsync(id);
    foreach (var membro in membros)
    {
        await _usuarioEquipeRepository.RemoveMembroAsync(id, membro.UsuarioId);
    }

    await _equipeRepository.DeleteAsync(id);
}
```

### **3. Método Auxiliar (Opcional)**
```csharp
private async Task<bool> EquipeTemReservasAtivasAsync(int equipeId)
{
    var reservasAtivas = await _reservaRepository.GetByEquipeAsync(equipeId);
    var reservasPendentes = await _reservaRepository.GetPendentesAsync(equipeId);
    var reservasAprovadas = await _reservaRepository.GetAprovadasAsync(equipeId);

    return reservasAtivas.Any() || reservasPendentes.Any() || reservasAprovadas.Any();
}
```

---

## 🧪 **CENÁRIOS DE TESTE NECESSÁRIOS**

### **❌ Cenário 1: Equipe com Reservas Ativas**
- [ ] Validação de autenticação passa
- [ ] Validação de administrador principal passa
- [ ] Validação de reservas ativas falha
- [ ] Lança exceção: "Não é possível excluir a equipe pois ela possui reservas ativas"

### **❌ Cenário 2: Equipe com Reservas Pendentes**
- [ ] Validação de autenticação passa
- [ ] Validação de administrador principal passa
- [ ] Validação de reservas pendentes falha
- [ ] Lança exceção: "Não é possível excluir a equipe pois ela possui reservas pendentes"

### **❌ Cenário 3: Equipe com Reservas Aprovadas**
- [ ] Validação de autenticação passa
- [ ] Validação de administrador principal passa
- [ ] Validação de reservas aprovadas falha
- [ ] Lança exceção: "Não é possível excluir a equipe pois ela possui reservas aprovadas"

### **❌ Cenário 4: Equipe sem Reservas**
- [ ] Validação de autenticação passa
- [ ] Validação de administrador principal passa
- [ ] Validação de reservas passa
- [ ] Exclusão é permitida

---

## 📊 **COMPONENTES ENVOLVIDOS**

### **Arquivos que Precisam ser Modificados:**
1. ❌ `src/ReservaPeriferico.Application/Services/EquipeService.cs` - Adicionar validação
2. ❌ `src/ReservaPeriferico.Web/Program.cs` - Verificar se IReservaRepository está registrado

### **Serviços que Precisam ser Utilizados:**
- ❌ `IReservaRepository` - Acesso aos métodos de verificação
- ❌ `GetByEquipeAsync()` - Todas as reservas da equipe
- ❌ `GetPendentesAsync()` - Reservas pendentes da equipe
- ❌ `GetAprovadasAsync()` - Reservas aprovadas da equipe

---

## 🎯 **COMPORTAMENTO ESPERADO**

### **Processo de Validação (Após Implementação):**
1. ✅ **Autenticação:** Verifica se usuário está logado
2. ✅ **Autoridade:** Verifica se é o administrador principal
3. 🆕 **Reservas:** Verifica se equipe tem reservas ativas/pendentes/aprovadas
4. ✅ **Limpeza:** Remove todos os membros da equipe
5. ✅ **Exclusão:** Remove a equipe principal

### **Exceções de Segurança (Após Implementação):**
- ✅ `UnauthorizedAccessException("Usuário não autenticado")`
- ✅ `UnauthorizedAccessException("Apenas o administrador principal pode excluir esta equipe")`
- 🆕 `InvalidOperationException("Não é possível excluir a equipe pois ela possui reservas ativas")`

---

## 🔒 **SEGURANÇA NECESSÁRIA**

### **Validações de Segurança (Após Implementação):**
1. ✅ **Autenticação obrigatória** - Usuário deve estar logado
2. ✅ **Autoridade principal** - Deve ser quem criou a equipe
3. 🆕 **Integridade de dados** - Não pode excluir com reservas ativas
4. ✅ **Validação no servidor** - Não pode ser burlado pelo frontend
5. ✅ **Exceções apropriadas** - Mensagens claras de erro

---

## ✅ **CONCLUSÃO**

### **Status: IMPLEMENTAÇÃO NECESSÁRIA** ❌

A **Tarefa 6** **NÃO está implementada** e precisa ser implementada para:

- ✅ **Prevenir exclusão** de equipes com reservas ativas
- ✅ **Manter integridade** referencial dos dados
- ✅ **Evitar dados órfãos** no sistema
- ✅ **Garantir consistência** entre equipes e reservas

### **Implementação Necessária:**
1. ❌ **Injetar IReservaRepository** no EquipeService
2. ❌ **Adicionar validação** de reservas no DeleteAsync
3. ❌ **Implementar exceções** apropriadas
4. ❌ **Testar cenários** de validação

### **Esta tarefa precisa ser implementada!** ❌

---

## 🚀 **PRÓXIMA TAREFA**

Após implementar a **Tarefa 6**, podemos partir para a **Tarefa 7: Validar exclusão de equipe com periféricos cadastrados**.

**Quer que eu implemente a Tarefa 6 agora?** 🚀

---

## 📅 Data de Análise
**Data:** 12/10/2025
**Tempo de análise:** 20 minutos
**Status:** IMPLEMENTAÇÃO NECESSÁRIA ❌

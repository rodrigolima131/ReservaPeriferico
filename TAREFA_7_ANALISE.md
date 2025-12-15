# 📝 TAREFA 7 - ANÁLISE DE IMPLEMENTAÇÃO

## ❌ TAREFA 7: Validar exclusão de equipe com periféricos cadastrados

### 📊 Status: **NÃO IMPLEMENTADA** ❌

### 🎯 Objetivo
No `EquipeService.DeleteAsync`, validar se a equipe possui periféricos cadastrados antes de permitir a exclusão, garantindo integridade referencial.

---

## 🔍 ANÁLISE COMPLETA

### ❌ **DESCOBERTA IMPORTANTE:**
A **Tarefa 7 NÃO está implementada!** A validação de periféricos cadastrados está ausente no método `DeleteAsync`.

### 📋 **IMPLEMENTAÇÃO ATUAL (INCOMPLETA):**

#### **Backend - EquipeService.cs (Validação de Segurança e Reservas)**

**Validação no DeleteAsync (ATUAL - após Tarefa 6):**
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

    // ✅ VALIDAÇÃO DE RESERVAS - Verificar se equipe tem QUALQUER reserva vinculada
    var todasReservas = await _reservaRepository.GetByEquipeAsync(id);
    if (todasReservas.Any())
    {
        throw new InvalidOperationException("Não é possível excluir a equipe pois ela possui reservas vinculadas");
    }

    // ❌ FALTA: VALIDAÇÃO DE PERIFÉRICOS CADASTRADOS
    // ❌ FALTA: Verificar se equipe tem periféricos cadastrados
    // ❌ FALTA: Verificar se equipe tem periféricos ativos
    // ❌ FALTA: Verificar se equipe tem periféricos inativos

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

### **✅ RELACIONAMENTO ENTRE ENTIDADES:**

#### **Periferico Entity:**
```csharp
public class Periferico
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    // ... outros campos ...
    public int EquipeId { get; set; }  // ✅ FK para Equipe
    public virtual Equipe Equipe { get; set; } = null!;
}
```

#### **Equipe Entity:**
```csharp
public class Equipe
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    // ... outros campos ...
    public virtual ICollection<Periferico> Perifericos { get; set; } = new List<Periferico>();  // ✅ Relacionamento 1:N
}
```

#### **Configuração do Entity Framework:**
```csharp
// ApplicationDbContext.cs
entity.HasOne(e => e.Equipe)
      .WithMany(e => e.Perifericos)
      .HasForeignKey(e => e.EquipeId)
      .OnDelete(DeleteBehavior.Restrict);  // ✅ Já impede exclusão no banco
```

**⚠️ IMPORTANTE:** O banco de dados já está configurado com `DeleteBehavior.Restrict`, o que significa que o banco **já impede** a exclusão de uma equipe se houver periféricos vinculados. No entanto, **é importante validar no código** para:
1. **Dar uma mensagem de erro mais clara e amigável ao usuário**
2. **Evitar que a exceção do banco seja propagada diretamente**
3. **Manter consistência com a validação de reservas (Tarefa 6)**

---

### **❌ DEPENDÊNCIAS AUSENTES:**

#### **IPerifericoRepository não tem método para buscar por EquipeId:**
```csharp
public interface IPerifericoRepository : IRepository<Periferico>
{
    Task<IEnumerable<Periferico>> GetByTipoAsync(string tipo);
    Task<IEnumerable<Periferico>> GetAtivosAsync();
    Task<Periferico?> GetByNumeroSerieAsync(string numeroSerie);
    Task<bool> NumeroSerieExistsAsync(string numeroSerie, int? excludeId = null);
    
    // ❌ FALTA: Task<IEnumerable<Periferico>> GetByEquipeIdAsync(int equipeId);
}
```

#### **EquipeService não tem acesso ao PerifericoRepository:**
```csharp
public class EquipeService : IEquipeService
{
    private readonly IEquipeRepository _equipeRepository;
    private readonly IUsuarioEquipeRepository _usuarioEquipeRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IReservaRepository _reservaRepository;  // ✅ Adicionado na Tarefa 6
    
    // ❌ FALTA: IPerifericoRepository _perifericoRepository;
    
    public EquipeService(
        IEquipeRepository equipeRepository, 
        IUsuarioEquipeRepository usuarioEquipeRepository, 
        IHttpContextAccessor httpContextAccessor,
        IReservaRepository reservaRepository  // ✅ Adicionado na Tarefa 6
        // ❌ FALTA: IPerifericoRepository perifericoRepository
    )
    {
        // ...
    }
}
```

---

## 📋 **O QUE PRECISA SER IMPLEMENTADO**

### **1. Adicionar método no IPerifericoRepository:**
```csharp
Task<IEnumerable<Periferico>> GetByEquipeIdAsync(int equipeId);
```

### **2. Implementar método no PerifericoRepository:**
```csharp
public async Task<IEnumerable<Periferico>> GetByEquipeIdAsync(int equipeId)
{
    return await _dbSet
        .Where(p => p.EquipeId == equipeId)
        .ToListAsync();
}
```

### **3. Injetar IPerifericoRepository no EquipeService:**
```csharp
private readonly IPerifericoRepository _perifericoRepository;

public EquipeService(
    IEquipeRepository equipeRepository, 
    IUsuarioEquipeRepository usuarioEquipeRepository, 
    IHttpContextAccessor httpContextAccessor,
    IReservaRepository reservaRepository,
    IPerifericoRepository perifericoRepository  // 🆕 ADICIONAR
)
{
    _equipeRepository = equipeRepository;
    _usuarioEquipeRepository = usuarioEquipeRepository;
    _httpContextAccessor = httpContextAccessor;
    _reservaRepository = reservaRepository;
    _perifericoRepository = perifericoRepository;  // 🆕 ADICIONAR
}
```

### **4. Adicionar validação no DeleteAsync:**
```csharp
// 🆕 VALIDAÇÃO DE PERIFÉRICOS - Verificar se equipe tem QUALQUER periférico vinculado
var todosPerifericos = await _perifericoRepository.GetByEquipeIdAsync(id);

if (todosPerifericos.Any())
{
    throw new InvalidOperationException("Não é possível excluir a equipe pois ela possui periféricos cadastrados");
}
```

---

## 🎯 **CRITÉRIOS DE ACEITE**

### ✅ **O que deve ser validado:**
1. ✅ Verificar se a equipe possui **QUALQUER periférico** cadastrado (ativo ou inativo)
2. ✅ Impedir exclusão se houver periféricos vinculados
3. ✅ Exibir mensagem de erro clara e amigável
4. ✅ Manter consistência com a validação de reservas (Tarefa 6)

### ❌ **O que NÃO precisa ser validado:**
- ❌ Não precisa diferenciar entre periféricos ativos e inativos (qualquer periférico impede exclusão)
- ❌ Não precisa verificar se o periférico está em uso (isso já é coberto pela validação de reservas)

---

## 📝 **ORDEM DE VALIDAÇÕES NO DeleteAsync**

A ordem das validações deve ser:
1. ✅ Verificar se equipe existe
2. ✅ Verificar se usuário está autenticado
3. ✅ Verificar se usuário é administrador principal
4. ✅ Verificar se equipe tem reservas vinculadas (Tarefa 6)
5. 🆕 **Verificar se equipe tem periféricos cadastrados (Tarefa 7)**
6. ✅ Remover membros
7. ✅ Excluir equipe

---

## 🔄 **COMPARAÇÃO COM TAREFA 6**

| Aspecto | Tarefa 6 (Reservas) | Tarefa 7 (Periféricos) |
|---------|---------------------|------------------------|
| **Repositório** | `IReservaRepository` | `IPerifericoRepository` |
| **Método de busca** | `GetByEquipeAsync(int equipeId)` | `GetByEquipeIdAsync(int equipeId)` - **PRECISA SER CRIADO** |
| **Validação** | Qualquer reserva (todos os status) | Qualquer periférico (ativo ou inativo) |
| **Mensagem de erro** | "Não é possível excluir a equipe pois ela possui reservas vinculadas" | "Não é possível excluir a equipe pois ela possui periféricos cadastrados" |
| **Ordem de validação** | Antes de periféricos | Depois de reservas |

---

## ✅ **CHECKLIST DE IMPLEMENTAÇÃO**

- [ ] 1. Adicionar método `GetByEquipeIdAsync(int equipeId)` na interface `IPerifericoRepository`
- [ ] 2. Implementar método `GetByEquipeIdAsync(int equipeId)` no `PerifericoRepository`
- [ ] 3. Injetar `IPerifericoRepository` no construtor do `EquipeService`
- [ ] 4. Adicionar campo privado `_perifericoRepository` no `EquipeService`
- [ ] 5. Adicionar validação de periféricos no método `DeleteAsync` (após validação de reservas)
- [ ] 6. Testar exclusão de equipe sem periféricos (deve funcionar)
- [ ] 7. Testar exclusão de equipe com periféricos ativos (deve bloquear)
- [ ] 8. Testar exclusão de equipe com periféricos inativos (deve bloquear)
- [ ] 9. Verificar mensagem de erro exibida ao usuário
- [ ] 10. Verificar que a validação ocorre antes da tentativa de exclusão no banco

---

## 📚 **ARQUIVOS QUE SERÃO MODIFICADOS**

1. **`src/ReservaPeriferico.Core/Interfaces/IPerifericoRepository.cs`**
   - Adicionar método `GetByEquipeIdAsync(int equipeId)`

2. **`src/ReservaPeriferico.Infrastructure/Repositories/PerifericoRepository.cs`**
   - Implementar método `GetByEquipeIdAsync(int equipeId)`

3. **`src/ReservaPeriferico.Application/Services/EquipeService.cs`**
   - Injetar `IPerifericoRepository` no construtor
   - Adicionar campo privado `_perifericoRepository`
   - Adicionar validação no método `DeleteAsync`

---

## 🎯 **RESUMO**

A **Tarefa 7** é similar à **Tarefa 6**, mas valida periféricos ao invés de reservas. A implementação segue o mesmo padrão:
1. Adicionar método de busca no repositório
2. Injetar repositório no serviço
3. Adicionar validação antes da exclusão

**Importante:** O banco de dados já impede a exclusão (via `DeleteBehavior.Restrict`), mas a validação no código é importante para:
- Mensagens de erro mais claras
- Consistência com a validação de reservas
- Melhor experiência do usuário


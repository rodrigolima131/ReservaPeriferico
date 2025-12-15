# 📊 Status de Reserva - Referência Completa

## 📋 Enum de Status

**Arquivo:** `src/ReservaPeriferico.Core/Enums/StatusReserva.cs`

```csharp
public enum StatusReserva
{
    Pendente = 1,
    Aprovada = 2,
    Rejeitada = 3,
    Cancelada = 4,
    EmUso = 5,
    Devolvida = 6,
    Expirada = 7
}
```

---

## 🔢 Códigos e Descrições

| Código | Status | Descrição | Quando é Gravado |
|--------|--------|-----------|------------------|
| **1** | Pendente | Aguardando aprovação | Ao criar uma nova reserva |
| **2** | Aprovada | Reserva aprovada pelo administrador | Quando administrador aprova |
| **3** | Rejeitada | Reserva rejeitada pelo administrador | Quando administrador rejeita |
| **4** | Cancelada | Reserva cancelada pelo usuário | Quando usuário cancela |
| **5** | Em Uso | Periférico em uso | Quando a data de início chega |
| **6** | Devolvida | Periférico devolvido | Quando periférico é devolvido |
| **7** | Expirada | Reserva expirada | Quando data final passa |

---

## 🗂️ Campos da Tabela `reserva`

### **Estrutura Completa:**

| Campo | Tipo | Obrigatório | Descrição |
|------|------|-------------|-----------|
| `id` | INT | ✅ | ID único da reserva |
| `usuario_id` | INT | ✅ | ID do usuário que solicitou |
| `periferico_id` | INT | ✅ | ID do periférico |
| `equipe_id` | INT | ✅ | ID da equipe |
| `data_inicio` | TIMESTAMP | ✅ | Data de início da reserva |
| `data_fim` | TIMESTAMP | ❌ | Data de fim da reserva (opcional) |
| `observacoes` | VARCHAR(500) | ❌ | Observações do usuário |
| **`status`** | INT | ✅ | **Status da reserva (1-7)** |
| `usuario_aprovador_id` | INT | ❌ | ID do administrador que aprovou/rejeitou |
| `data_aprovacao` | TIMESTAMP | ❌ | Data da aprovação |
| `motivo_rejeicao` | VARCHAR(500) | ❌ | Motivo da rejeição |
| `data_cadastro` | TIMESTAMP | ✅ | Data de criação |
| `data_atualizacao` | TIMESTAMP | ❌ | Data da última atualização |
| `data_devolucao` | TIMESTAMP | ❌ | Data da devolução |

---

## 🔄 Fluxo de Status

```
NOVA RESERVA
    ↓
(Pendente = 1) ← Padrão ao criar
    ↓
    ├─→ Aprovada = 2 (administrador)
    │       ↓
    │       ├─→ Em Uso = 5 (quando data_inicio chega)
    │       │       ↓
    │       │       └─→ Devolvida = 6 (quando devolve)
    │       │
    │       └─→ Cancelada = 4 (usuário cancela)
    │
    ├─→ Rejeitada = 3 (administrador)
    │
    ├─→ Cancelada = 4 (usuário cancela)
    │
    └─→ Expirada = 7 (data_fim passou)
```

---

## 📝 Status em Diferentes Contextos

### **1. Pendente (1) - Criar Reserva**
**Arquivo:** `ReservaService.SolicitarReservaAsync()`

```csharp
var reserva = new Reserva
{
    UsuarioId = usuarioId,
    PerifericoId = solicitarReservaDto.PerifericoId,
    EquipeId = periferico.EquipeId,
    DataInicio = solicitarReservaDto.DataInicio,
    DataFim = solicitarReservaDto.DataFim,
    Observacoes = solicitarReservaDto.Observacoes,
    Status = StatusReserva.Pendente, // ✅ Código 1
    DataCadastro = DateTime.UtcNow
};
```

### **2. Aprovada (2) - Aprovar Reserva**
**Arquivo:** `ReservaService.AprovarReservaAsync()`

```csharp
reserva.Status = StatusReserva.Aprovada; // ✅ Código 2
reserva.UsuarioAprovadorId = usuarioAprovadorId;
reserva.DataAprovacao = DateTime.UtcNow.ToLocalTime();
```

### **3. Rejeitada (3) - Rejeitar Reserva**
**Arquivo:** `ReservaService.AprovarReservaAsync()`

```csharp
reserva.Status = StatusReserva.Rejeitada; // ✅ Código 3
reserva.UsuarioAprovadorId = usuarioAprovadorId;
reserva.DataAprovacao = DateTime.UtcNow.ToLocalTime();
reserva.MotivoRejeicao = aprovarReservaDto.Motivo;
```

### **4. Cancelada (4) - Cancelar Reserva**
**Arquivo:** `ReservaService.CancelarReservaAsync()`

```csharp
reserva.Status = StatusReserva.Cancelada; // ✅ Código 4
reserva.DataAtualizacao = DateTime.UtcNow;
```

### **5. Em Uso (5) - Status Ativo**
Gerenciado automaticamente quando a data de início chega.

### **6. Devolvida (6) - Devolver Periférico**
Gerenciado quando o periférico é devolvido.

### **7. Expirada (7) - Reserva Expirada**
Gerenciado automaticamente quando a data final passa.

---

## 🔍 Consultas SQL Úteis

### **Ver todas as reservas por status:**
```sql
SELECT status, COUNT(*) as quantidade
FROM reserva
GROUP BY status
ORDER BY status;
```

### **Ver reservas pendentes:**
```sql
SELECT * FROM reserva WHERE status = 1;
```

### **Ver reservas aprovadas:**
```sql
SELECT * FROM reserva WHERE status = 2;
```

### **Ver reservas rejeitadas:**
```sql
SELECT * FROM reserva WHERE status = 3;
```

### **Ver reservas canceladas:**
```sql
SELECT * FROM reserva WHERE status = 4;
```

### **Ver todas as reservas com descrição do status:**
```sql
SELECT 
    id,
    usuario_id,
    periferico_id,
    equipe_id,
    data_inicio,
    data_fim,
    CASE status
        WHEN 1 THEN 'Pendente'
        WHEN 2 THEN 'Aprovada'
        WHEN 3 THEN 'Rejeitada'
        WHEN 4 THEN 'Cancelada'
        WHEN 5 THEN 'Em Uso'
        WHEN 6 THEN 'Devolvida'
        WHEN 7 THEN 'Expirada'
    END as status_texto,
    status,
    observacoes,
    data_cadastro
FROM reserva
ORDER BY data_cadastro DESC;
```

---

## 📊 Valores no Banco de Dados

| Status | Código no Banco |
|--------|-----------------|
| Pendente | 1 |
| Aprovada | 2 |
| Rejeitada | 3 |
| Cancelada | 4 |
| Em Uso | 5 |
| Devolvida | 6 |
| Expirada | 7 |

---

## ✅ Conclusão

O campo `status` na tabela `reserva` armazena um **INTEGER** que corresponde ao enum `StatusReserva`.

**Valores possíveis:** 1, 2, 3, 4, 5, 6, ou 7

**Status padrão ao criar:** 1 (Pendente)

**Status mais comum:** 1 (Pendente) → 2 (Aprovada) → 5 (Em Uso) → 6 (Devolvida)

---

## 📅 Data de Referência
**Data:** 12/10/2025
**Arquivo:** `src/ReservaPeriferico.Core/Enums/StatusReserva.cs`

---

# 🎯 DEFINIÇÃO DE "RESERVA ATIVA"

## 🔍 O que é uma Reserva Ativa?

Uma **reserva ativa** é uma reserva que:
1. ✅ Está **Aprovada** (status = 2)
2. ✅ **Ainda não venceu** (data_fim >= hoje)
3. ✅ **Não foi devolvida** (data_devolucao IS NULL)

### **Código de Definição:**

**Arquivo:** `src/ReservaPeriferico.Infrastructure/Repositories/ReservaRepository.cs` (linha 53-64)

```csharp
public async Task<IEnumerable<Reserva>> GetAtivasAsync()
{
    var hoje = DateTime.Today;
    return await _dbSet
        .Include(r => r.Usuario)
        .Include(r => r.Periferico)
        .Include(r => r.Equipe)
        .Include(r => r.UsuarioAprovador)
        .Where(r => r.Status == StatusReserva.Aprovada &&      // ✅ Status = 2 (Aprovada)
                   r.DataFim >= hoje &&                          // ✅ Ainda não venceu
                   r.DataDevolucao == null)                       // ✅ Não foi devolvida
        .OrderByDescending(r => r.DataCadastro)
        .ToListAsync();
}
```

### **Traduzindo para SQL:**

```sql
SELECT * 
FROM reserva 
WHERE status = 2                    -- Aprovada
  AND data_fim >= CURRENT_DATE      -- Não venceu ainda
  AND data_devolucao IS NULL;       -- Não devolvida
```

---

## 📊 Campos Relevantes para "Reserva Ativa"

| Campo | Valor Esperado |
|-------|----------------|
| `status` | **2** (Aprovada) |
| `data_fim` | **>= hoje** |
| `data_devolucao` | **NULL** |

---

## 🔄 Quando uma Reserva deixa de ser "Ativa"?

Uma reserva deixa de ser ativa quando:

1. ✅ **Status muda** de Aprovada (2) para outro status
2. ✅ **Data fim passa** (data_fim < hoje)
3. ✅ **É devolvida** (data_devolucao recebe uma data)

---

## 💡 Por que isso é importante?

As **reservas ativas** são usadas para:
- ✅ Contar quantos periféricos estão reservados
- ✅ Verificar disponibilidade de periféricos
- ✅ Calcular ocupação dos periféricos
- ✅ Validar se uma equipe pode ser excluída (Tarefa 6)

---

## 🧪 Exemplo Prático

### **Cenário: Reserva Ativa**
```sql
INSERT INTO reserva (
    usuario_id, 
    periferico_id, 
    equipe_id, 
    data_inicio, 
    data_fim, 
    status,
    data_cadastro
) VALUES (
    2,                    -- usuário
    1,                    -- periférico
    6,                    -- equipe
    '2025-10-15',         -- início
    '2025-10-25',         -- fim (ainda no futuro)
    2,                    -- Aprovada
    NOW()
);

-- ✅ Esta é uma RESERVA ATIVA
-- Status = 2 (Aprovada)
-- Data fim = 25/10/2025 (ainda não passou)
-- Data devolução = NULL
```

### **Cenário: Reserva NÃO Ativa**
```sql
-- Opção 1: Status Pendente
status = 1  -- ❌ NÃO é ativa (status Pendente)

-- Opção 2: Data fim passou
data_fim = '2025-10-01'  -- ❌ NÃO é ativa (já venceu)

-- Opção 3: Foi devolvida
data_devolucao = '2025-10-10'  -- ❌ NÃO é ativa (já devolvida)
```

---

## 🔍 Query SQL para Ver Reservas Ativas

```sql
SELECT 
    id,
    usuario_id,
    periferico_id,
    equipe_id,
    data_inicio,
    data_fim,
    status,
    data_devolucao,
    -- Mostrar quanto tempo falta
    CASE 
        WHEN data_fim >= CURRENT_DATE THEN 
            CONCAT((data_fim - CURRENT_DATE), ' dias')
        ELSE 
            'Vencida'
    END as tempo_restante
FROM reserva 
WHERE status = 2                    -- Aprovada
  AND data_fim >= CURRENT_DATE     -- Não venceu
  AND data_devolucao IS NULL       -- Não devolvida
ORDER BY data_inicio;
```

---

## ✅ Resumo

### **Reserva Ativa =**
- Status: **2** (Aprovada)
- Data fim: **>= hoje**
- Data devolução: **NULL**

### **Status:**
- **2** (Aprovada) - Reserva ativa (se data fim >= hoje e não devolvida)
- **5** (Em Uso) - Pode ser considerado ativa também
- **6** (Devolvida) - NÃO é ativa
- Qualquer outro status - NÃO é ativa


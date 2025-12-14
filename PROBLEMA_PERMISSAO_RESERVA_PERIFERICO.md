# 🔍 Problema: "Usuário não tem permissão para reservar este periférico"

## 📊 Situação

**Usuário:**
- Nome: Alexander Dias Brito
- ID: 2
- Equipe criada: ID 6 (EQUIPE ONE PIECE)

**Erro:**
```
Erro na reserva: Usuário não tem permissão para reservar este periférico
```

---

## 🔍 Causa Raiz

A validação de permissão verifica se o usuário está vinculado à **equipe do periférico**:

```csharp
public async Task<bool> UsuarioPodeReservarAsync(int usuarioId, int perifericoId)
{
    var periferico = await _perifericoRepository.GetByIdAsync(perifericoId);
    if (periferico == null) return false;

    // ✅ Verifica se o usuário está vinculado à equipe do periférico
    var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, periferico.EquipeId);
    return usuarioEquipe != null && usuarioEquipe.Usuario.Ativo;
}
```

**O que acontece:**
1. ✅ Você criou a equipe ID 6
2. ❌ Todos os periféricos pertencem à **equipe ID 1** (hardcoded)
3. ❌ Você **NÃO** está vinculado à equipe ID 1
4. ❌ Validação falha porque você não pertence à equipe do periférico

---

## 🛠️ Soluções Possíveis

### **Solução 1: Adicionar Você à Equipe 1 (Temporária)**

Se os periféricos existentes pertencem à equipe 1, você pode se adicionar como membro:

```sql
INSERT INTO usuario_equipe (usuario_id, equipe_id, is_administrador, data_entrada)
VALUES (2, 1, true, NOW());
```

### **Solução 2: Adicionar Periféricos à Sua Equipe (Recomendada)**

Para usar os periféricos na sua equipe (ID 6), você precisa:

1. **Criar novos periféricos** na sua equipe
2. **OU** alterar a equipe dos periféricos existentes:

```sql
-- Ver todos os periféricos
SELECT id, nome, equipe_id FROM periferico;

-- Mover um periférico para sua equipe
UPDATE periferico 
SET equipe_id = 6 
WHERE id = <ID_DO_PERIFERICO>;
```

### **Solução 3: Corrigir PerifericoForm para Permitir Seleção de Equipe**

O formulário de criação de periféricos está com `EquipeId` hardcoded. Precisamos:

1. Permitir selecionar a equipe ao criar periférico
2. Ou pré-selecionar a equipe do usuário logado

---

## 🎯 Recomendação

**IMEDIATA:** Adicione-se à equipe 1 para poder reservar os periféricos existentes:

```sql
INSERT INTO usuario_equipe (usuario_id, equipe_id, is_administrador, data_entrada)
VALUES (2, 1, true, NOW());
```

**LONGO PRAZO:** Corrigir o formulário de periféricos para permitir selecionar a equipe.

---

## 📝 Código da Validação

**Arquivo:** `src/ReservaPeriferico.Application/Services/ReservaService.cs`

```csharp
public async Task<bool> UsuarioPodeReservarAsync(int usuarioId, int perifericoId)
{
    var periferico = await _perifericoRepository.GetByIdAsync(perifericoId);
    if (periferico == null) return false;

    // Verifica se o usuário está vinculado à equipe do periférico
    var usuarioEquipe = await _usuarioEquipeRepository.GetByUsuarioAndEquipeAsync(usuarioId, periferico.EquipeId);
    return usuarioEquipe != null && usuarioEquipe.Usuario.Ativo;
}
```

**Chamada:**
```csharp
if (!await UsuarioPodeReservarAsync(usuarioId, solicitarReservaDto.PerifericoId))
    throw new ReservaException("Usuário não tem permissão para reservar este periférico");
```

---

## ✅ Como Verificar Suas Equipes

Execute para ver quais equipes você pertence:

```sql
SELECT 
    ue.usuario_id,
    ue.equipe_id,
    e.nome as equipe_nome,
    ue.is_administrador,
    ue.data_entrada
FROM usuario_equipe ue
JOIN equipe e ON ue.equipe_id = e.id
WHERE ue.usuario_id = 2;
```

---

## 📅 Data
**Data:** 12/10/2025  
**Status:** DOCUMENTADO - Aguardando ação do usuário


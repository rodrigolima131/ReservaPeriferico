# Guia de Cadastro de Periféricos (ReservaPeriferico)

## Objetivo da funcionalidade
Permitir que o **administrador de uma equipe** cadastre os periféricos disponíveis para **reserva** pelos membros da sua equipe, assegurando **controle, rastreabilidade, disponibilidade** e **não-duplicidade** de identificação dentro da equipe.

## Perfis e Permissões
- **Administrador da equipe**
  - Pode **cadastrar**, **editar**, **desativar** e **excluir** periféricos da **sua** equipe.
  - Não pode excluir periféricos com **reservas ativas**; nesses casos, apenas **desativar**.
- **Membros da equipe**
  - Podem **listar/visualizar** periféricos da sua equipe e **solicitar reserva**.
  - Não podem criar/editar/excluir periféricos.

> Regra de escopo: **cada periférico pertence a uma única equipe**. Não há compartilhamento entre equipes (restrição atual).

## Campos do Cadastro e Regras
**Obrigatórios**
- `Nome` (ex.: “Mouse Logitech”, “Monitor 24”)
- `Categoria` (ex.: Mouse, Teclado, Monitor, Headset…)
- `NumeroIdentificacao` (ex.: patrimônio/código interno/etiqueta)
- `EstadoConservacao` (**enum**: `Novo`, `Usado`, `Danificado`)
- `QuantidadeTotal` (>= 1)

**Opcionais**
- `NumeroSerie` (se aplicável; útil para rastreabilidade)
- `Observacoes` (texto livre)

**Validações**
- Obrigatórios **não podem** estar em branco.
- **Duplicidade proibida** dentro da mesma equipe:
  - `EquipeId + NumeroIdentificacao` deve ser **único**.
  - Se informado, `EquipeId + NumeroSerie` também deve ser **único**.
- `QuantidadeTotal >= 1`. Edição que reduza abaixo de reservas futuras deve ser bloqueada.

## Status do Periférico
O sistema controla o `Status` com atualizações **automáticas** conforme uso/estoque e **manuais** pelo admin (para ativar/desativar).

- `Ativo` → **pode ser reservado** (visível a membros/admin).
- `Inativo` → **não pode ser reservado**, **visível só ao admin**.
- `EmUso` → há **reserva(s) corrente(s)** consumindo parte/total do estoque.
- `Indisponivel` → **sem disponibilidade** (ex.: `QuantidadeDisponivel = 0`) ou **avariado**.

**Transições (resumo)**
- `Ativo → EmUso`: ao iniciar reserva (quando há consumo de estoque).
- `EmUso → Ativo`: ao encerrar reservas e restabelecer disponibilidade.
- `Ativo/EmUso → Inativo`: ação do admin (bloquear se houver reservas ativas; com futuras, orientar desativar).
- Para qualquer estado → `Indisponivel`: disponibilidade zero **ou** `Danificado`.

> Na tela de reserva, **somente** periféricos `Ativo` com `QuantidadeDisponivel > 0` devem aparecer.

## Modelagem de Dados (EF Core / PostgreSQL)
**Tabela:** `Periferico`
- `Id` (PK, UUID)
- `EquipeId` (FK → `Equipe.Id`, not null)
- `Nome` (varchar, not null)
- `Categoria` (varchar, not null)
- `NumeroIdentificacao` (varchar, not null)
- `NumeroSerie` (varchar, null)
- `EstadoConservacao` (smallint/int enum: `Novo=0`, `Usado=1`, `Danificado=2`)
- `Status` (smallint/int enum: `Ativo=0`, `Inativo=1`, `EmUso=2`, `Indisponivel=3`)
- `QuantidadeTotal` (int, not null, >=1)
- `QuantidadeReservada` (int, not null, default 0)
- `Observacoes` (text, null)
- Audit: `CriadoEm`, `CriadoPor`, `AtualizadoEm`, `AtualizadoPor`

**Índices/Constraints**
- `UNIQUE (EquipeId, NumeroIdentificacao)`
- `UNIQUE (EquipeId, NumeroSerie)` (parcial: `NumeroSerie IS NOT NULL`)
- Busca: `(EquipeId, Categoria)`, `(EquipeId, Status)`, `(EquipeId, Nome)`

**Cálculos/Derivações**
- `QuantidadeDisponivel = QuantidadeTotal - QuantidadeReservada`
- Status automático:
  - `Danificado` ⇒ `Indisponivel`
  - `QuantidadeDisponivel = 0` ⇒ `Indisponivel` (ou `EmUso` se houver reservas correntes)
  - Senão, manter `Ativo`/`Inativo` conforme flag/admin

## Fluxo de Cadastro (Aplicação)
1. **Autorização**: verificar se usuário é **admin da equipe**.
2. **Validação**: obrigatórios, formatos e limites.
3. **Duplicidade**: checar unique `(EquipeId, NumeroIdentificacao)` e, se houver, `(EquipeId, NumeroSerie)`.
4. **Persistência**: salvar `Periferico` com:
   - `Status = Ativo` por padrão (se `EstadoConservacao ≠ Danificado`)
   - `QuantidadeReservada = 0`
5. **Retorno**: DTO criado com `Id`.

**Edição**
- Bloquear alterações que **quebrem reservas futuras** (ex.: reduzir `QuantidadeTotal` abaixo do já reservado).
- Permitir alterar `EstadoConservacao`; se `Danificado`, setar `Status = Indisponivel`.

**Exclusão**
- **Proibida** se houver **reservas ativas ou futuras**.
- Caso contrário, permitir exclusão (ideal: **soft delete**) e/ou `Inativo`.

**Desativação**
- Com **reservas futuras**, bloquear exclusão e permitir **desativar** (`Status = Inativo`), impedindo novas reservas.

## Endpoints (sugestão REST)
- `POST /equipes/{equipeId}/perifericos` — criar (admin).
- `GET /equipes/{equipeId}/perifericos?nome=&categoria=&status=&page=&pageSize=` — listar com filtros (membros/admin).
- `GET /equipes/{equipeId}/perifericos/{id}` — detalhe.
- `PUT /equipes/{equipeId}/perifericos/{id}` — editar (regras de reservas).
- `DELETE /equipes/{equipeId}/perifericos/{id}` — excluir (sem reservas ativas/futuras).
- `PATCH /equipes/{equipeId}/perifericos/{id}/status` — ativar/desativar (admin).

**Códigos/Erros esperados**
- `400` validação (campos, quantidade)
- `401/403` autorização (não admin / fora da equipe)
- `404` não encontrado
- `409` conflito (duplicidade; reservas impedem ação)

## Padrões de Nomeação (Migrations/Entities)
- Migrations:
  - `CriarTabelaPeriferico`
  - `AjustarPerifericoAdicionarNumeroSerie`
  - `AjustarPerifericoIndicesUnicos`
  - `SeedCategoriasPeriferico` (se aplicável)
- Enums (C#): `EstadoConservacao`, `StatusPeriferico`
- Tabela: `Periferico` (singular), FK: `EquipeId`

## Exemplo de Entidade (EF Core) — Resumo
```csharp
public class Periferico
{
    public Guid Id { get; set; }
    public Guid EquipeId { get; set; }
    public string Nome { get; set; } = default!;
    public string Categoria { get; set; } = default!;
    public string NumeroIdentificacao { get; set; } = default!;
    public string? NumeroSerie { get; set; }
    public EstadoConservacao EstadoConservacao { get; set; }
    public StatusPeriferico Status { get; set; }
    public int QuantidadeTotal { get; set; }
    public int QuantidadeReservada { get; set; }
    public string? Observacoes { get; set; }
    public DateTime CriadoEm { get; set; }
    public string CriadoPor { get; set; } = default!;
    public DateTime? AtualizadoEm { get; set; }
    public string? AtualizadoPor { get; set; }
}
```

**Mapeamento (índices/unique - Npgsql)**
```csharp
builder.HasIndex(p => new { p.EquipeId, p.NumeroIdentificacao }).IsUnique();
builder.HasIndex(p => new { p.EquipeId, p.NumeroSerie })
       .IsUnique()
       .HasFilter(""NumeroSerie" IS NOT NULL");
```

## UI/UX (Critérios práticos)
- **Cadastro (admin)**: obrigatórios destacados; mensagens inline; máscara/sugestão para `NumeroIdentificacao`.
- **Listagem**: filtros por `Nome`, `Categoria`, `Status`; paginação.
- **Visibilidade**:
  - Membros veem apenas `Ativo` com **quantidade disponível**.
  - Admin vê todos, com **status** e **contadores de disponibilidade**.
- **Ações**:
  - Admin: **Editar**, **Desativar**, **Excluir** (com impedimentos quando houver reservas).

## Critérios de Aceite (DoD)
- Tela de **cadastro** acessível **apenas** ao admin.
- **Validação** de obrigatórios e **duplicidade** por equipe.
- **Persistência** correta (índices/uniques).
- **Listagem** por equipe com filtros de `nome`, `categoria` e `status`.
- **Edição/Exclusão** visíveis apenas ao admin; exclusão **bloqueada** com reservas ativas/futuras.
- **Status** exibido na listagem; `Ativo` com disponibilidade **pode ser reservado**.
- `Inativo`, `EmUso` e `Indisponivel` **não aparecem** para membros na tela de reserva.

## Observações Técnicas
- **Modelagem**: `Periferico` com FK `EquipeId`.
- **Estoque por quantidade**: usar `QuantidadeTotal` e **derivar** `QuantidadeReservada` das reservas ativas/futuras (manter consistência em operações de reserva/cancelamento).
- **Enums** para `EstadoConservacao` e `Status`.
- **Atualização automática de status**:
  - Ao criar/editar/cancelar reservas, **recalcular** `QuantidadeReservada` e **atualizar** `Status`.
  - Ao marcar `Danificado`, setar `Status = Indisponivel`.
- **Migrations (EF Core)**: toda alteração de schema via migrations; **não** editar migrations já aplicadas; gerar **script idempotente** em produção.

## Casos de Teste (mínimos)
- Criar periférico válido (admin) → `201 Created`.
- Tentar criar com obrigatórios em branco → `400`.
- Tentar criar duplicado (`NumeroIdentificacao`) na mesma equipe → `409`.
- Listar (membro) → vê apenas `Ativo` com disponibilidade.
- Editar reduzindo `QuantidadeTotal` abaixo do já reservado → `400`.
- Excluir com reservas ativas/futuras → `409`.
- Desativar com reservas futuras → permitido (bloqueia novas reservas).
- Alterar `EstadoConservacao = Danificado` → `Status = Indisponivel`.

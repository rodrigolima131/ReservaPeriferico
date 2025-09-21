# Guia — Confirmação de Devolução por Administrador

## Descrição
Atualmente, quando um equipamento reservado é devolvido, ele automaticamente volta a ficar disponível para novas reservas. Para maior controle, será necessário que o **administrador da equipe** confirme a devolução **antes** que o periférico volte a ficar disponível no sistema. Isso garante que apenas após a validação do administrador o equipamento seja liberado novamente.

## Objetivo
Introduzir o status **Devolução Pendente** e um fluxo de aprovação pelo administrador para que a liberação de disponibilidade ocorra **somente após confirmação**.

---

## Requisitos Funcionais

### Fluxo de Devolução
- Quando o usuário solicitar devolução de um equipamento, deve ser **criada uma solicitação de devolução**.
- O status da reserva **não** deve ser alterado automaticamente para “Finalizada/Devolvida”.

### Confirmação do Administrador
- O administrador da equipe deve **visualizar** as solicitações de devolução pendentes no **gerenciamento de reservas**.
- O administrador poderá **Confirmar** ou **Recusar** a devolução.
- **Apenas após Confirmação**, o equipamento volta a ficar **disponível** para novas reservas.

### Notificações
- Usuário deve ser **notificado por e-mail** quando sua devolução for **confirmada** ou **recusada**.
- Administrador deve ser **notificado** quando houver uma **nova solicitação de devolução pendente**.

---

## Estados e Transições (Reserva)

### Novos/Atualizados Status
- `Ativa` — reserva em vigor.
- `DevolucaoPendente` — usuário solicitou devolução; aguardando confirmação do admin.
- `Finalizada` — devidamente devolvida e confirmada.
- `Recusada` — solicitação de devolução recusada (reserva pode permanecer ativa ou exigir ação do usuário).

### Regras de Transição
- `Ativa → DevolucaoPendente`: usuário solicita devolução.
- `DevolucaoPendente → Finalizada`: admin **Confirma**; liberar disponibilidade do periférico.
- `DevolucaoPendente → Ativa` **ou** `Recusada`: admin **Recusa**; manter equipamento como não disponível para terceiros até orientação definida (padrão: volta a `Ativa`).

> Observação: a disponibilidade do periférico **só aumenta** quando a reserva vai para `Finalizada`.

---

## UI/UX (Pontos Principais)
- **Usuário (minhas reservas)**
  - Botão **Solicitar Devolução** quando a reserva estiver `Ativa`.
  - Exibir status `DevolucaoPendente` e mensagem “Aguardando confirmação do administrador”.
  - Exibir notificação de resultado (confirmado/recusado).
- **Administrador (gerenciar reservas)**
  - Nova aba/filtro **“Devoluções Pendentes”**.
  - Ações por item: **Confirmar** | **Recusar** (com observação opcional).
  - Lista com colunas: Usuário, Periférico, Período, Status atual, Ações.

---

## Modelagem de Dados (sugestão EF Core)
- **Reserva**
  - `StatusReserva` (enum): incluir `DevolucaoPendente`, `Finalizada`, `Recusada`.
  - `DataSolicitacaoDevolucao` (datetime, nullable)
  - `DataConfirmacaoDevolucao` (datetime, nullable)
  - `JustificativaRecusa` (text, nullable)
- **Periferico**
  - Recalcular `QuantidadeReservada` **apenas** na confirmação (transição para `Finalizada`).

**Índices/Consistência**
- Garantir que alterações de status sejam **atômicas** (transação) para manter coerência entre `Reserva` e disponibilidade do `Periferico`.

---

## Endpoints (sugestão REST)

### Usuário
- `POST /reservas/{reservaId}/devolucao`  
  - Ação: Solicitar devolução.  
  - Regras: Reserva deve estar `Ativa`. Seta `Status = DevolucaoPendente` e `DataSolicitacaoDevolucao = now()`.
- `GET /reservas?meu-usuario=true&status=DevolucaoPendente|Ativa|Finalizada|Recusada`  
  - Ação: Listar reservas do usuário com filtro de status.

### Administrador
- `GET /equipes/{equipeId}/reservas?status=DevolucaoPendente`  
  - Ação: Listar solicitações pendentes.
- `POST /reservas/{reservaId}/confirmar-devolucao`  
  - Ação: **Confirmar** devolução.  
  - Efeitos: `Status = Finalizada`, `DataConfirmacaoDevolucao = now()`, **liberar disponibilidade** (decrementar `QuantidadeReservada`).
- `POST /reservas/{reservaId}/recusar-devolucao`  
  - Ação: **Recusar** devolução (corpo com `JustificativaRecusa`).  
  - Efeitos: `Status = Ativa` **ou** `Recusada` conforme política; **não** liberar disponibilidade.

**Erros esperados**
- `400` (estado inválido, p.ex. solicitar devolução fora de `Ativa`)
- `401/403` (sem autenticação/permissão)
- `404` (reserva não encontrada ou fora da equipe)
- `409` (conflito de concorrência/transição já processada)

---

## Notificações (E-mail)
- **Ao criar Devolução Pendente** (para admin): assunto “Nova solicitação de devolução”; incluir dados da reserva e link para gerenciamento.
- **Ao Confirmar** (para usuário): assunto “Devolução confirmada”; status muda para `Finalizada`.
- **Ao Recusar** (para usuário): assunto “Devolução recusada”; incluir `JustificativaRecusa`.
- Preferir **envio assíncrono** via fila/worker para não impactar a UX.

---

## Tarefas (Checklist Técnico)

1. **Domain/Enum**
   - Adicionar `DevolucaoPendente` (e `Recusada` se ainda não existir) em `StatusReserva`.
2. **Fluxo de Solicitação**
   - Endpoint para solicitar devolução; validação de estado; persistir datas/campos.
3. **Gestão do Admin**
   - Listagem com filtro `DevolucaoPendente` no gerenciamento de reservas.
   - Ações **Confirmar/Recusar** com regras e transações consistentes.
4. **Disponibilidade do Periférico**
   - Atualizar `QuantidadeReservada` e disponibilidade **somente** na confirmação.
5. **Notificações**
   - Integração com serviço de e-mail (eventos: `SolicitacaoCriada`, `DevolucaoConfirmada`, `DevolucaoRecusada`).
6. **Testes**
   - Unidade (domínio/serviços), integração (endpoints/transações) e e2e (fluxo completo).
7. **Migrations**
   - Adicionar colunas de auditoria/apoio e novos valores de enum; versionar via EF Core.
8. **Concorrência**
   - Implementar controle otimista (rowversion/timestamp) para evitar duplo processamento.
9. **Auditoria**
   - Registrar usuário e data/hora em cada transição de status.

---

## Critérios de Aceite (DoD)
- Equipamento **só** volta a ficar disponível **após confirmação** do administrador.
- Solicitações de devolução aparecem no **gerenciamento de reservas** do admin.
- Usuário é **notificado** sobre o resultado da devolução (confirmada/recusada).
- Administrador recebe **notificação** a cada nova solicitação.
- Testes cobrindo cenários de **devolução confirmada** e **recusada**, incluindo concorrência.
- Logs/auditoria registram todas as transições (quem, quando, de quê para quê).

---

## Considerações de Segurança & UX
- Apenas **admins da equipe** podem confirmar/recusar devolução dessa equipe.
- Exibir mensagens claras de estado ao usuário.
- Garantir idempotência nos endpoints de confirmação/recusa (repetições não devem duplicar efeitos).
- Monitorar taxa de erro no envio de e-mails; re-tentativas via fila.

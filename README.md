# Sistema de Reserva de Periféricos

Sistema web para gerenciamento de reservas de periféricos desenvolvido em .NET 9 com Blazor Server e MudBlazor.

## Funcionalidades

### Gerenciamento de Periféricos
- **Cadastro de Periféricos**: Adicione novos periféricos ao sistema com informações completas
- **Edição de Periféricos**: Modifique dados de periféricos existentes
- **Exclusão de Periféricos**: Remova periféricos do sistema
- **Ativação/Desativação**: Controle o status dos periféricos
- **Filtros e Busca**: Encontre periféricos por tipo, status ou texto livre
- **Validações**: Sistema robusto de validação de dados

### Campos do Periférico
- **Nome** (obrigatório): Nome do periférico
- **Tipo** (obrigatório): Categoria do periférico (Mouse, Teclado, Monitor, etc.)
- **Marca** (obrigatório): Fabricante do periférico
- **Modelo** (opcional): Modelo específico
- **Número de Série** (obrigatório): Identificador único do periférico
- **Descrição** (opcional): Detalhes adicionais
- **Status**: Ativo/Inativo para controle de disponibilidade

## Tecnologias Utilizadas

- **.NET 9**: Framework principal
- **Blazor Server**: Interface web
- **MudBlazor**: Componentes UI modernos
- **Entity Framework Core**: ORM para acesso a dados
- **PostgreSQL**: Banco de dados
- **Clean Architecture**: Arquitetura limpa com separação de responsabilidades

## Estrutura do Projeto

```
ReservaPeriferico/
├── src/
│   ├── ReservaPeriferico.Core/          # Entidades e interfaces
│   ├── ReservaPeriferico.Application/    # Serviços e DTOs
│   ├── ReservaPeriferico.Infrastructure/ # Repositórios e contexto
│   └── ReservaPeriferico.Web/           # Interface Blazor
```

## Como rodar localmente

1. Instale o .NET 9 ou superior
2. Configure o banco PostgreSQL e ajuste a connection string em `appsettings.json`
3. Execute:
   ```sh
   dotnet ef database update --startup-project src/ReservaPeriferico.Web
   dotnet run --project src/ReservaPeriferico.Web
   ```

## Estrutura
- `src/ReservaPeriferico.Core` - Entidades e regras de negócio
- `src/ReservaPeriferico.Application` - Serviços e DTOs
- `src/ReservaPeriferico.Infrastructure` - Persistência e migrations
- `src/ReservaPeriferico.Web` - Interface Blazor Server

## Contribuição
Pull requests são bem-vindos!

## Funcionalidades da Tela de Periféricos

### Interface Principal
- **Botão "Novo Periférico"**: Abre diálogo para cadastro
- **Botão "Atualizar"**: Recarrega a lista de periféricos
- **Filtros**: Por tipo e status
- **Busca**: Texto livre em todos os campos

### Tabela de Periféricos
- **Colunas**: Nome, Tipo, Marca/Modelo, Número de Série, Status, Data Cadastro
- **Ações**: Editar, Excluir, Ativar/Desativar
- **Paginação**: Navegação entre páginas
- **Ordenação**: Clique nos cabeçalhos para ordenar

### Diálogo de Cadastro/Edição
- **Campos obrigatórios**: Nome, Tipo, Marca, Número de Série
- **Validação em tempo real**: Feedback imediato de erros
- **Validação de unicidade**: Número de série deve ser único
- **Switch de status**: Controle de ativação/desativação

## Validações Implementadas

- **Nome**: Obrigatório, máximo 100 caracteres
- **Tipo**: Obrigatório, máximo 50 caracteres
- **Marca**: Obrigatório, máximo 50 caracteres
- **Número de Série**: Obrigatório, máximo 20 caracteres, único no sistema
- **Descrição**: Opcional, máximo 500 caracteres

## Tipos de Periféricos Disponíveis

- Mouse
- Teclado
- Monitor
- Headset
- Webcam
- Impressora
- Scanner
- Outro

## Recursos de UX

- **Tooltips informativos**: Dicas sobre cada campo e ação
- **Feedback visual**: Cores e ícones para status
- **Confirmações**: Diálogos de confirmação para ações destrutivas
- **Notificações**: Snackbars para feedback de operações
- **Loading states**: Indicadores de carregamento
- **Responsividade**: Interface adaptável a diferentes tamanhos de tela

## Arquitetura

O projeto segue os princípios da Clean Architecture:

- **Core**: Entidades e interfaces de domínio
- **Application**: Casos de uso e regras de negócio
- **Infrastructure**: Implementação de repositórios e acesso a dados
- **Web**: Interface do usuário

## 🆘 Suporte

Para dúvidas ou problemas:
1. Verifique a documentação
2. Abra uma issue no repositório
3. Entre em contato com a equipe de desenvolvimento 

# Estrutura banco de dados 

Visão Geral

Usamos Entity Framework Core (EF Core) para versionar e aplicar o esquema do banco por meio de Migrations.
Todas as alterações de schema (criar/alterar/excluir tabelas, colunas, índices, FKs) devem passar por migrations — nada de alterar direto no banco.

- Provider alvo: (PostgreSQL)
- Projeto das migrations: ReservaPeriferico.Infrastructure
- Pasta padrão: ReservaPeriferico.Infrastructure/Migrations
- Tabela de histórico: __EFMigrationsHistory (controla o que já foi aplicado)

# 1) Gerar a migration usar exemplo: 
    executar comando do da pasta raiz ReservaPeriferico
        dotnet ef migrations add NomeMigration `
        --project src/ReservaPeriferico.Infrastructure `
        --startup-project src/ReservaPeriferico.Web `
        --output-dir Migrations

# 2)Para remover ultimo migration, usar exemplo: 
    executar comando do da pasta raiz ReservaPeriferico
        dotnet ef migrations remove `
        --project src/ReservaPeriferico.Infrastructure `
        --startup-project src/ReservaPeriferico.Web

*Exemplos para criação do migration*
- Navegue para o caminho ReservaPeriferico.Infrastructure/Migrations.
- Use padrões de nomeclatura para a criação dos migrations.
- Nomes: 
    - Ajuste para realizar o ajuste de alguma tabela. exemplo: AjusteEquipe. 
    - Criar para realizar a criação de alguma tabela. exemplo: CriarTabelaEquipe. 
- Em caso de dúvidas, valide os migrations existentes. 

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReservaPeriferico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class criacaoTabelaParametro : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "parametro",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    chave = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    valor = table.Column<string>(type: "text", nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    tipo = table.Column<int>(type: "integer", nullable: false),
                    data_criacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    usuario_atualizacao = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_parametro", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_parametro_chave",
                table: "parametro",
                column: "chave",
                unique: true);

            migrationBuilder.Sql(@"
                -- Inserir parâmetros de Email
                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailSmtpServer', 'smtp.gmail.com', 'Servidor SMTP para envio de emails', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailSmtpServer');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailSmtpPort', '587', 'Porta do servidor SMTP', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailSmtpPort');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailUsername', '', 'Usuário para autenticação SMTP', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailUsername');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailPassword', '', 'Senha para autenticação SMTP', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailPassword');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailEnableSsl', 'true', 'Habilitar SSL para conexão SMTP', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailEnableSsl');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailFromEmail', '', 'Email remetente padrão', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailFromEmail');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailFromName', 'Sistema de Reservas', 'Nome do remetente padrão', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailFromName');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'EmailTimeout', '30000', 'Timeout em milissegundos para envio de email', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'EmailTimeout');

                -- Inserir parâmetros de Notificação
                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'NotificacaoHabilitada', 'true', 'Habilitar sistema de notificações', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'NotificacaoHabilitada');

                INSERT INTO parametro (chave, valor, descricao, ativo, data_criacao, tipo)
                SELECT 'NotificacaoTimeout', '30000', 'Timeout para notificações em milissegundos', true, Now(), 0
                WHERE NOT EXISTS (SELECT 1 FROM parametro WHERE chave = 'NotificacaoTimeout');
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "parametro");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReservaPeriferico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjustesEquipe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "equipe",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    usuario_administrador_id = table.Column<int>(type: "integer", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_equipe", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario_equipe",
                columns: table => new
                {
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    equipe_id = table.Column<int>(type: "integer", nullable: false),
                    is_administrador = table.Column<bool>(type: "boolean", nullable: false),
                    data_entrada = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario_equipe", x => new { x.usuario_id, x.equipe_id });
                    table.ForeignKey(
                        name: "FK_usuario_equipe_equipe_equipe_id",
                        column: x => x.equipe_id,
                        principalTable: "equipe",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_usuario_equipe_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_equipe_nome",
                table: "equipe",
                column: "nome",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_equipe_equipe_id",
                table: "usuario_equipe",
                column: "equipe_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "usuario_equipe");

            migrationBuilder.DropTable(
                name: "equipe");
        }
    }
}

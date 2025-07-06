using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ReservaPeriferico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "periferico",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    descricao = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    tipo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    marca = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    modelo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    numero_serie = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_periferico", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usuario",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    matricula = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    departamento = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    cargo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ativo = table.Column<bool>(type: "boolean", nullable: false),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_usuario", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reserva",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    usuario_id = table.Column<int>(type: "integer", nullable: false),
                    periferico_id = table.Column<int>(type: "integer", nullable: false),
                    data_inicio = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_fim = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    observacoes = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    data_cadastro = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    data_atualizacao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    data_devolucao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_reserva", x => x.id);
                    table.ForeignKey(
                        name: "FK_reserva_periferico_periferico_id",
                        column: x => x.periferico_id,
                        principalTable: "periferico",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_reserva_usuario_usuario_id",
                        column: x => x.usuario_id,
                        principalTable: "usuario",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "usuario",
                columns: new[] { "id", "ativo", "cargo", "data_atualizacao", "data_cadastro", "departamento", "email", "matricula", "nome" },
                values: new object[] { 1, true, "Desenvolvedor", null, new DateTime(2025, 7, 6, 21, 9, 21, 610, DateTimeKind.Utc).AddTicks(1293), "TI", "teste@empresa.com", "TESTE001", "Usuário Teste" });

            migrationBuilder.CreateIndex(
                name: "IX_periferico_numero_serie",
                table: "periferico",
                column: "numero_serie",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_periferico_id",
                table: "reserva",
                column: "periferico_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_usuario_id",
                table: "reserva",
                column: "usuario_id");

            migrationBuilder.CreateIndex(
                name: "IX_usuario_email",
                table: "usuario",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_usuario_matricula",
                table: "usuario",
                column: "matricula",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reserva");

            migrationBuilder.DropTable(
                name: "periferico");

            migrationBuilder.DropTable(
                name: "usuario");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaPeriferico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AjusteGerenciarReserva : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "data_fim",
                table: "reserva",
                type: "timestamp with time zone",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "data_aprovacao",
                table: "reserva",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "equipe_id",
                table: "reserva",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "motivo_rejeicao",
                table: "reserva",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "status",
                table: "reserva",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "usuario_aprovador_id",
                table: "reserva",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "equipe_id",
                table: "periferico",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_reserva_equipe_id",
                table: "reserva",
                column: "equipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_reserva_usuario_aprovador_id",
                table: "reserva",
                column: "usuario_aprovador_id");

            migrationBuilder.CreateIndex(
                name: "IX_periferico_equipe_id",
                table: "periferico",
                column: "equipe_id");

            migrationBuilder.CreateIndex(
                name: "IX_equipe_usuario_administrador_id",
                table: "equipe",
                column: "usuario_administrador_id");

            migrationBuilder.AddForeignKey(
                name: "FK_equipe_usuario_usuario_administrador_id",
                table: "equipe",
                column: "usuario_administrador_id",
                principalTable: "usuario",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_periferico_equipe_equipe_id",
                table: "periferico",
                column: "equipe_id",
                principalTable: "equipe",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reserva_equipe_equipe_id",
                table: "reserva",
                column: "equipe_id",
                principalTable: "equipe",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_reserva_usuario_usuario_aprovador_id",
                table: "reserva",
                column: "usuario_aprovador_id",
                principalTable: "usuario",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
            
            migrationBuilder.InsertData(
                table: "equipe",
                columns: new[] { "id", "data_atualizacao", "data_cadastro", "descricao", "nome", "usuario_administrador_id" },
                values: new object[] { 1, null, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Equipe responsável pelo desenvolvimento de software", "Equipe PDV", 1 });

            migrationBuilder.InsertData(
                table: "usuario_equipe",
                columns: new[] { "equipe_id", "usuario_id", "data_entrada", "is_administrador" },
                values: new object[] { 1, 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_equipe_usuario_usuario_administrador_id",
                table: "equipe");

            migrationBuilder.DropForeignKey(
                name: "FK_periferico_equipe_equipe_id",
                table: "periferico");

            migrationBuilder.DropForeignKey(
                name: "FK_reserva_equipe_equipe_id",
                table: "reserva");

            migrationBuilder.DropForeignKey(
                name: "FK_reserva_usuario_usuario_aprovador_id",
                table: "reserva");

            migrationBuilder.DropIndex(
                name: "IX_reserva_equipe_id",
                table: "reserva");

            migrationBuilder.DropIndex(
                name: "IX_reserva_usuario_aprovador_id",
                table: "reserva");

            migrationBuilder.DropIndex(
                name: "IX_periferico_equipe_id",
                table: "periferico");

            migrationBuilder.DropIndex(
                name: "IX_equipe_usuario_administrador_id",
                table: "equipe");

            migrationBuilder.DropColumn(
                name: "data_aprovacao",
                table: "reserva");

            migrationBuilder.DropColumn(
                name: "equipe_id",
                table: "reserva");

            migrationBuilder.DropColumn(
                name: "motivo_rejeicao",
                table: "reserva");

            migrationBuilder.DropColumn(
                name: "status",
                table: "reserva");

            migrationBuilder.DropColumn(
                name: "usuario_aprovador_id",
                table: "reserva");

            migrationBuilder.DropColumn(
                name: "equipe_id",
                table: "periferico");

            migrationBuilder.AlterColumn<DateTime>(
                name: "data_fim",
                table: "reserva",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "timestamp with time zone",
                oldNullable: true);

            migrationBuilder.DeleteData(
                table: "usuario_equipe",
                keyColumns: new[] { "equipe_id", "usuario_id" },
                keyValues: new object[] { 1, 1 });

            migrationBuilder.DeleteData(
                table: "equipe",
                keyColumn: "id",
                keyValue: 1);

        }
    }
}

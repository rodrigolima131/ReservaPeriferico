using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReservaPeriferico.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuario",
                keyColumn: "id",
                keyValue: 1,
                column: "data_cadastro",
                value: new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "usuario",
                keyColumn: "id",
                keyValue: 1,
                column: "data_cadastro",
                value: new DateTime(2025, 7, 6, 21, 9, 21, 610, DateTimeKind.Utc).AddTicks(1293));
        }
    }
}

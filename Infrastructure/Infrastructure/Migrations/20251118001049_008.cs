using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _008 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportesDanos_Equipos_EquipoId",
                table: "ReportesDanos");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipoId",
                table: "ReportesDanos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "SalaId",
                table: "ReportesDanos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Tipo",
                table: "ReportesDanos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ReportesDanos_SalaId",
                table: "ReportesDanos",
                column: "SalaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportesDanos_Equipos_EquipoId",
                table: "ReportesDanos",
                column: "EquipoId",
                principalTable: "Equipos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReportesDanos_Salas_SalaId",
                table: "ReportesDanos",
                column: "SalaId",
                principalTable: "Salas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReportesDanos_Equipos_EquipoId",
                table: "ReportesDanos");

            migrationBuilder.DropForeignKey(
                name: "FK_ReportesDanos_Salas_SalaId",
                table: "ReportesDanos");

            migrationBuilder.DropIndex(
                name: "IX_ReportesDanos_SalaId",
                table: "ReportesDanos");

            migrationBuilder.DropColumn(
                name: "SalaId",
                table: "ReportesDanos");

            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "ReportesDanos");

            migrationBuilder.AlterColumn<Guid>(
                name: "EquipoId",
                table: "ReportesDanos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ReportesDanos_Equipos_EquipoId",
                table: "ReportesDanos",
                column: "EquipoId",
                principalTable: "Equipos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

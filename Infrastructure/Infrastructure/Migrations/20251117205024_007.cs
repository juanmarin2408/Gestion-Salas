using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _007 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AsignadoAId",
                table: "Equipos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaBloqueo",
                table: "Equipos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoBloqueo",
                table: "Equipos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PrioridadBloqueo",
                table: "Equipos",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Equipos_AsignadoAId",
                table: "Equipos",
                column: "AsignadoAId");

            migrationBuilder.AddForeignKey(
                name: "FK_Equipos_Usuarios_AsignadoAId",
                table: "Equipos",
                column: "AsignadoAId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Equipos_Usuarios_AsignadoAId",
                table: "Equipos");

            migrationBuilder.DropIndex(
                name: "IX_Equipos_AsignadoAId",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "AsignadoAId",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "FechaBloqueo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "MotivoBloqueo",
                table: "Equipos");

            migrationBuilder.DropColumn(
                name: "PrioridadBloqueo",
                table: "Equipos");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateHistoricoConfronto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HistoricoDesafios",
                columns: table => new
                {
                    IdHistoricoConfronto = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDesafio = table.Column<int>(type: "int", nullable: false),
                    IdDesafiante = table.Column<int>(type: "int", nullable: false),
                    IdDesafiado = table.Column<int>(type: "int", nullable: false),
                    IdVencedor = table.Column<int>(type: "int", nullable: false),
                    DataConfronto = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoricoDesafios", x => x.IdHistoricoConfronto);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HistoricoDesafios");
        }
    }
}

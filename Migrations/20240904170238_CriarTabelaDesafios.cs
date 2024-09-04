using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class CriarTabelaDesafios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Desafios",
                columns: table => new
                {
                    IdDesafio = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDesafiante = table.Column<int>(type: "int", nullable: false),
                    IdDesafiado = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    NivelAtual = table.Column<int>(type: "int", nullable: false),
                    DesafianteJogando = table.Column<bool>(type: "bit", nullable: false),
                    InsigniasDesafiante = table.Column<int>(type: "int", nullable: false),
                    InsigniasDesafiado = table.Column<int>(type: "int", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Finalizado = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desafios", x => x.IdDesafio);
                    table.ForeignKey(
                        name: "FK_Desafios_Trilhas_IdTrilha",
                        column: x => x.IdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Desafios_Usuarios_IdDesafiado",
                        column: x => x.IdDesafiado,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                    table.ForeignKey(
                        name: "FK_Desafios_Usuarios_IdDesafiante",
                        column: x => x.IdDesafiante,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Desafios_IdDesafiado",
                table: "Desafios",
                column: "IdDesafiado");

            migrationBuilder.CreateIndex(
                name: "IX_Desafios_IdDesafiante",
                table: "Desafios",
                column: "IdDesafiante");

            migrationBuilder.CreateIndex(
                name: "IX_Desafios_IdTrilha",
                table: "Desafios",
                column: "IdTrilha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Desafios");
        }
    }
}

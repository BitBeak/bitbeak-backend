using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddUsuarioNivelConcluido : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "QuestoesRespondidas");

            migrationBuilder.CreateTable(
                name: "UsuarioNiveisConcluidos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioNiveisConcluidos", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuarioTrilhasConcluidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    DataConclusao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTrilhasConcluidas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsuarioNiveisConcluidos");

            migrationBuilder.DropTable(
                name: "UsuarioTrilhasConcluidas");

            migrationBuilder.CreateTable(
                name: "QuestoesRespondidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdQuestao = table.Column<int>(type: "int", nullable: false),
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestoesRespondidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestoesRespondidas_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestoesRespondidas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_QuestoesRespondidas_IdQuestao",
                table: "QuestoesRespondidas",
                column: "IdQuestao");

            migrationBuilder.CreateIndex(
                name: "IX_QuestoesRespondidas_IdUsuario",
                table: "QuestoesRespondidas",
                column: "IdUsuario");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarAmizades : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CodigoDeAmizade",
                table: "Usuarios",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Amizades",
                columns: table => new
                {
                    IdAmizade = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdAmigo = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Amizades", x => x.IdAmizade);
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_IdAmigo",
                        column: x => x.IdAmigo,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Amizades_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_IdAmigo",
                table: "Amizades",
                column: "IdAmigo");

            migrationBuilder.CreateIndex(
                name: "IX_Amizades_IdUsuario",
                table: "Amizades",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Amizades");

            migrationBuilder.DropTable(
                name: "UsuarioNiveisConcluidos");

            migrationBuilder.DropTable(
                name: "UsuarioTrilhasConcluidas");

            migrationBuilder.DropColumn(
                name: "CodigoDeAmizade",
                table: "Usuarios");

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

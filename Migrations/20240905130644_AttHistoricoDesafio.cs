using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AttHistoricoDesafio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_HistoricoDesafios_IdDesafiado",
                table: "HistoricoDesafios",
                column: "IdDesafiado");

            migrationBuilder.CreateIndex(
                name: "IX_HistoricoDesafios_IdDesafiante",
                table: "HistoricoDesafios",
                column: "IdDesafiante");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoDesafios_Usuarios_IdDesafiado",
                table: "HistoricoDesafios",
                column: "IdDesafiado",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricoDesafios_Usuarios_IdDesafiante",
                table: "HistoricoDesafios",
                column: "IdDesafiante",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoDesafios_Usuarios_IdDesafiado",
                table: "HistoricoDesafios");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricoDesafios_Usuarios_IdDesafiante",
                table: "HistoricoDesafios");

            migrationBuilder.DropIndex(
                name: "IX_HistoricoDesafios_IdDesafiado",
                table: "HistoricoDesafios");

            migrationBuilder.DropIndex(
                name: "IX_HistoricoDesafios_IdDesafiante",
                table: "HistoricoDesafios");
        }
    }
}

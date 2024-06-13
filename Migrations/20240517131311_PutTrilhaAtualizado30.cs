using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class PutTrilhaAtualizado30 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes");

            migrationBuilder.DropIndex(
                name: "IX_Questoes_NivelIdNivel",
                table: "Questoes");

            migrationBuilder.DropColumn(
                name: "NivelIdNivel",
                table: "Questoes");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Questoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_IdNivel",
                table: "Questoes",
                column: "IdNivel");

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_NiveisTrilha_IdNivel",
                table: "Questoes",
                column: "IdNivel",
                principalTable: "NiveisTrilha",
                principalColumn: "IdNivel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_IdNivel",
                table: "Questoes");

            migrationBuilder.DropIndex(
                name: "IX_Questoes_IdNivel",
                table: "Questoes");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Questoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "NivelIdNivel",
                table: "Questoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel");

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel",
                principalTable: "NiveisTrilha",
                principalColumn: "IdNivel",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

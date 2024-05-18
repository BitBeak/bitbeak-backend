using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class MakeSolucaoEsperadaOptional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes");

            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_Trilhas_ModelTrilhaIdTrilha",
                table: "Questoes");

            migrationBuilder.DropIndex(
                name: "IX_Questoes_ModelTrilhaIdTrilha",
                table: "Questoes");

            migrationBuilder.DropColumn(
                name: "ModelTrilhaIdTrilha",
                table: "Questoes");

            migrationBuilder.AlterColumn<string>(
                name: "SolucaoEsperada",
                table: "Questoes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "NivelIdNivel",
                table: "Questoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel",
                principalTable: "NiveisTrilha",
                principalColumn: "IdNivel");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes");

            migrationBuilder.AlterColumn<string>(
                name: "SolucaoEsperada",
                table: "Questoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "NivelIdNivel",
                table: "Questoes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ModelTrilhaIdTrilha",
                table: "Questoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_ModelTrilhaIdTrilha",
                table: "Questoes",
                column: "ModelTrilhaIdTrilha");

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel",
                principalTable: "NiveisTrilha",
                principalColumn: "IdNivel",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_Trilhas_ModelTrilhaIdTrilha",
                table: "Questoes",
                column: "ModelTrilhaIdTrilha",
                principalTable: "Trilhas",
                principalColumn: "IdTrilha");
        }
    }
}

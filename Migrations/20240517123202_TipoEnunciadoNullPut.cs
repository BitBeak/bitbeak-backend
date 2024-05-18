using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class TipoEnunciadoNullPut : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes");

            migrationBuilder.AlterColumn<int>(
                name: "Tipo",
                table: "Questoes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel",
                principalTable: "NiveisTrilha",
                principalColumn: "IdNivel",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
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
    }
}

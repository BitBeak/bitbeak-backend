using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSolucaoEsperadaToQuestoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SolucaoEsperada",
                table: "Questoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Lacunas",
                columns: table => new
                {
                    IdLacuna = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ColunaA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ColunaB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdQuestao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lacunas", x => x.IdLacuna);
                    table.ForeignKey(
                        name: "FK_Lacunas_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OpcoesRespostas",
                columns: table => new
                {
                    IdOpcao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correta = table.Column<bool>(type: "bit", nullable: false),
                    IdQuestao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcoesRespostas", x => x.IdOpcao);
                    table.ForeignKey(
                        name: "FK_OpcoesRespostas_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lacunas_IdQuestao",
                table: "Lacunas",
                column: "IdQuestao");

            migrationBuilder.CreateIndex(
                name: "IX_OpcoesRespostas_IdQuestao",
                table: "OpcoesRespostas",
                column: "IdQuestao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lacunas");

            migrationBuilder.DropTable(
                name: "OpcoesRespostas");

            migrationBuilder.DropColumn(
                name: "SolucaoEsperada",
                table: "Questoes");
        }
    }
}

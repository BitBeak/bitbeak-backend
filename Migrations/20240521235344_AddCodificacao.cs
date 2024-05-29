using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddCodificacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CasoTeste",
                columns: table => new
                {
                    IdCasoTeste = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Entrada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SaidaEsperada = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdQuestao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CasoTeste", x => x.IdCasoTeste);
                    table.ForeignKey(
                        name: "FK_CasoTeste_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CasoTeste_IdQuestao",
                table: "CasoTeste",
                column: "IdQuestao");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CasoTeste");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class PerguntasEspeciais : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PerguntasEspeciais",
                columns: table => new
                {
                    IdPerguntaEspecial = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    Enunciado = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PerguntasEspeciais", x => x.IdPerguntaEspecial);
                    table.ForeignKey(
                        name: "FK_PerguntasEspeciais_NiveisTrilha_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "NiveisTrilha",
                        principalColumn: "IdNivel");
                    table.ForeignKey(
                        name: "FK_PerguntasEspeciais_Trilhas_IdTrilha",
                        column: x => x.IdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha");
                });

            migrationBuilder.CreateTable(
                name: "OpcaoRespostaEspecial",
                columns: table => new
                {
                    IdOpcao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Texto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Correta = table.Column<bool>(type: "bit", nullable: false),
                    IdPerguntaEspecial = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OpcaoRespostaEspecial", x => x.IdOpcao);
                    table.ForeignKey(
                        name: "FK_OpcaoRespostaEspecial_PerguntasEspeciais_IdPerguntaEspecial",
                        column: x => x.IdPerguntaEspecial,
                        principalTable: "PerguntasEspeciais",
                        principalColumn: "IdPerguntaEspecial",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OpcaoRespostaEspecial_IdPerguntaEspecial",
                table: "OpcaoRespostaEspecial",
                column: "IdPerguntaEspecial");

            migrationBuilder.CreateIndex(
                name: "IX_PerguntasEspeciais_IdNivel",
                table: "PerguntasEspeciais",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_PerguntasEspeciais_IdTrilha",
                table: "PerguntasEspeciais",
                column: "IdTrilha");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OpcaoRespostaEspecial");

            migrationBuilder.DropTable(
                name: "PerguntasEspeciais");
        }
    }
}

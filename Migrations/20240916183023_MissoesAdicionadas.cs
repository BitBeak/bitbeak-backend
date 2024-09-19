using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class MissoesAdicionadas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Missoes",
                columns: table => new
                {
                    IdMissao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Descricao = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TipoMissao = table.Column<int>(type: "int", nullable: false),
                    Objetivo = table.Column<int>(type: "int", nullable: false),
                    RecompensaPenas = table.Column<int>(type: "int", nullable: false),
                    RecompensaExperiencia = table.Column<int>(type: "int", nullable: false),
                    Inicial = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missoes", x => x.IdMissao);
                });

            migrationBuilder.CreateTable(
                name: "ProgressoMissoes",
                columns: table => new
                {
                    IdMissaoAtiva = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdMissao = table.Column<int>(type: "int", nullable: false),
                    ProgressoAtual = table.Column<int>(type: "int", nullable: false),
                    Completa = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressoMissoes", x => x.IdMissaoAtiva);
                    table.ForeignKey(
                        name: "FK_ProgressoMissoes_Missoes_IdMissao",
                        column: x => x.IdMissao,
                        principalTable: "Missoes",
                        principalColumn: "IdMissao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProgressoMissoes_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProgressoMissoes_IdMissao",
                table: "ProgressoMissoes",
                column: "IdMissao");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressoMissoes_IdUsuario",
                table: "ProgressoMissoes",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProgressoMissoes");

            migrationBuilder.DropTable(
                name: "Missoes");
        }
    }
}

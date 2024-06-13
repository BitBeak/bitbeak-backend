using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddTrilhasQuestoesUsuarios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Experiencia",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Nivel",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Penas",
                table: "Usuarios",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Trilhas",
                columns: table => new
                {
                    IdTrilha = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Descricao = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trilhas", x => x.IdTrilha);
                });

            migrationBuilder.CreateTable(
                name: "NiveisTrilha",
                columns: table => new
                {
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisTrilha", x => x.IdNivel);
                    table.ForeignKey(
                        name: "FK_NiveisTrilha_Trilhas_IdTrilha",
                        column: x => x.IdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosTrilhasProgresso",
                columns: table => new
                {
                    IdProgresso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    Experiencia = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosTrilhasProgresso", x => x.IdProgresso);
                    table.ForeignKey(
                        name: "FK_UsuariosTrilhasProgresso_Trilhas_IdTrilha",
                        column: x => x.IdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuariosTrilhasProgresso_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questoes",
                columns: table => new
                {
                    IdQuestao = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Enunciado = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    IdNivel = table.Column<int>(type: "int", nullable: false),
                    NivelIdNivel = table.Column<int>(type: "int", nullable: false),
                    ModelTrilhaIdTrilha = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questoes", x => x.IdQuestao);
                    table.ForeignKey(
                        name: "FK_Questoes_NiveisTrilha_NivelIdNivel",
                        column: x => x.NivelIdNivel,
                        principalTable: "NiveisTrilha",
                        principalColumn: "IdNivel",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Questoes_Trilhas_ModelTrilhaIdTrilha",
                        column: x => x.ModelTrilhaIdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha");
                });

            migrationBuilder.CreateIndex(
                name: "IX_NiveisTrilha_IdTrilha",
                table: "NiveisTrilha",
                column: "IdTrilha");

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_ModelTrilhaIdTrilha",
                table: "Questoes",
                column: "ModelTrilhaIdTrilha");

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_NivelIdNivel",
                table: "Questoes",
                column: "NivelIdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTrilhasProgresso_IdTrilha",
                table: "UsuariosTrilhasProgresso",
                column: "IdTrilha");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosTrilhasProgresso_IdUsuario",
                table: "UsuariosTrilhasProgresso",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Questoes");

            migrationBuilder.DropTable(
                name: "UsuariosTrilhasProgresso");

            migrationBuilder.DropTable(
                name: "NiveisTrilha");

            migrationBuilder.DropTable(
                name: "Trilhas");

            migrationBuilder.DropColumn(
                name: "Experiencia",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Nivel",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "Penas",
                table: "Usuarios");
        }
    }
}

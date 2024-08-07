using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NiveisUsuario",
                columns: table => new
                {
                    IdNivelUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NivelUsuario = table.Column<int>(type: "int", nullable: false),
                    ExperienciaNecessaria = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NiveisUsuario", x => x.IdNivelUsuario);
                });

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
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenhaCriptografada = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetToken = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PasswordResetTokenExpiry = table.Column<DateTime>(type: "datetime2", nullable: true),
                    NivelUsuario = table.Column<int>(type: "int", nullable: false),
                    ExperienciaUsuario = table.Column<int>(type: "int", nullable: false),
                    Penas = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateTable(
                name: "NiveisTrilha",
                columns: table => new
                {
                    IdNivel = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    NivelName = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "UsuarioTrilhaProgresso",
                columns: table => new
                {
                    IdProgresso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdTrilha = table.Column<int>(type: "int", nullable: false),
                    NivelUsuario = table.Column<int>(type: "int", nullable: false),
                    ExperienciaUsuario = table.Column<int>(type: "int", nullable: false),
                    Penas = table.Column<int>(type: "int", nullable: false),
                    Erros = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioTrilhaProgresso", x => x.IdProgresso);
                    table.ForeignKey(
                        name: "FK_UsuarioTrilhaProgresso_Trilhas_IdTrilha",
                        column: x => x.IdTrilha,
                        principalTable: "Trilhas",
                        principalColumn: "IdTrilha",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioTrilhaProgresso_Usuarios_IdUsuario",
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
                    IdNivel = table.Column<int>(type: "int", nullable: true),
                    SolucaoEsperada = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questoes", x => x.IdQuestao);
                    table.ForeignKey(
                        name: "FK_Questoes_NiveisTrilha_IdNivel",
                        column: x => x.IdNivel,
                        principalTable: "NiveisTrilha",
                        principalColumn: "IdNivel");
                });

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
                name: "OpcoesResposta",
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
                    table.PrimaryKey("PK_OpcoesResposta", x => x.IdOpcao);
                    table.ForeignKey(
                        name: "FK_OpcoesResposta_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestoesRespondidas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    IdQuestao = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestoesRespondidas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestoesRespondidas_Questoes_IdQuestao",
                        column: x => x.IdQuestao,
                        principalTable: "Questoes",
                        principalColumn: "IdQuestao",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestoesRespondidas_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lacunas_IdQuestao",
                table: "Lacunas",
                column: "IdQuestao");

            migrationBuilder.CreateIndex(
                name: "IX_NiveisTrilha_IdTrilha",
                table: "NiveisTrilha",
                column: "IdTrilha");

            migrationBuilder.CreateIndex(
                name: "IX_OpcoesResposta_IdQuestao",
                table: "OpcoesResposta",
                column: "IdQuestao");

            migrationBuilder.CreateIndex(
                name: "IX_Questoes_IdNivel",
                table: "Questoes",
                column: "IdNivel");

            migrationBuilder.CreateIndex(
                name: "IX_QuestoesRespondidas_IdQuestao",
                table: "QuestoesRespondidas",
                column: "IdQuestao");

            migrationBuilder.CreateIndex(
                name: "IX_QuestoesRespondidas_IdUsuario",
                table: "QuestoesRespondidas",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioTrilhaProgresso_IdTrilha",
                table: "UsuarioTrilhaProgresso",
                column: "IdTrilha");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioTrilhaProgresso_IdUsuario",
                table: "UsuarioTrilhaProgresso",
                column: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lacunas");

            migrationBuilder.DropTable(
                name: "NiveisUsuario");

            migrationBuilder.DropTable(
                name: "OpcoesResposta");

            migrationBuilder.DropTable(
                name: "QuestoesRespondidas");

            migrationBuilder.DropTable(
                name: "UsuarioTrilhaProgresso");

            migrationBuilder.DropTable(
                name: "Questoes");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "NiveisTrilha");

            migrationBuilder.DropTable(
                name: "Trilhas");
        }
    }
}

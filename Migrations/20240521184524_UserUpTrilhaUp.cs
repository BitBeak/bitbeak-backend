using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class UserUpTrilhaUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ModelUsuarioTrilhaProgresso_Trilhas_IdTrilha",
                table: "ModelUsuarioTrilhaProgresso");

            migrationBuilder.DropForeignKey(
                name: "FK_ModelUsuarioTrilhaProgresso_Usuarios_IdUsuario",
                table: "ModelUsuarioTrilhaProgresso");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ModelUsuarioTrilhaProgresso",
                table: "ModelUsuarioTrilhaProgresso");

            migrationBuilder.RenameTable(
                name: "ModelUsuarioTrilhaProgresso",
                newName: "UsuarioTrilhaProgresso");

            migrationBuilder.RenameColumn(
                name: "Nivel",
                table: "Usuarios",
                newName: "NivelUsuario");

            migrationBuilder.RenameColumn(
                name: "Experiencia",
                table: "Usuarios",
                newName: "ExperienciaUsuario");

            migrationBuilder.RenameColumn(
                name: "Nivel",
                table: "UsuarioTrilhaProgresso",
                newName: "Penas");

            migrationBuilder.RenameColumn(
                name: "Experiencia",
                table: "UsuarioTrilhaProgresso",
                newName: "NivelUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ModelUsuarioTrilhaProgresso_IdUsuario",
                table: "UsuarioTrilhaProgresso",
                newName: "IX_UsuarioTrilhaProgresso_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_ModelUsuarioTrilhaProgresso_IdTrilha",
                table: "UsuarioTrilhaProgresso",
                newName: "IX_UsuarioTrilhaProgresso_IdTrilha");

            migrationBuilder.AddColumn<int>(
                name: "ExperienciaUsuario",
                table: "UsuarioTrilhaProgresso",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsuarioTrilhaProgresso",
                table: "UsuarioTrilhaProgresso",
                column: "IdProgresso");

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

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioTrilhaProgresso_Trilhas_IdTrilha",
                table: "UsuarioTrilhaProgresso",
                column: "IdTrilha",
                principalTable: "Trilhas",
                principalColumn: "IdTrilha",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UsuarioTrilhaProgresso_Usuarios_IdUsuario",
                table: "UsuarioTrilhaProgresso",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioTrilhaProgresso_Trilhas_IdTrilha",
                table: "UsuarioTrilhaProgresso");

            migrationBuilder.DropForeignKey(
                name: "FK_UsuarioTrilhaProgresso_Usuarios_IdUsuario",
                table: "UsuarioTrilhaProgresso");

            migrationBuilder.DropTable(
                name: "NiveisUsuario");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsuarioTrilhaProgresso",
                table: "UsuarioTrilhaProgresso");

            migrationBuilder.DropColumn(
                name: "ExperienciaUsuario",
                table: "UsuarioTrilhaProgresso");

            migrationBuilder.RenameTable(
                name: "UsuarioTrilhaProgresso",
                newName: "ModelUsuarioTrilhaProgresso");

            migrationBuilder.RenameColumn(
                name: "NivelUsuario",
                table: "Usuarios",
                newName: "Nivel");

            migrationBuilder.RenameColumn(
                name: "ExperienciaUsuario",
                table: "Usuarios",
                newName: "Experiencia");

            migrationBuilder.RenameColumn(
                name: "Penas",
                table: "ModelUsuarioTrilhaProgresso",
                newName: "Nivel");

            migrationBuilder.RenameColumn(
                name: "NivelUsuario",
                table: "ModelUsuarioTrilhaProgresso",
                newName: "Experiencia");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioTrilhaProgresso_IdUsuario",
                table: "ModelUsuarioTrilhaProgresso",
                newName: "IX_ModelUsuarioTrilhaProgresso_IdUsuario");

            migrationBuilder.RenameIndex(
                name: "IX_UsuarioTrilhaProgresso_IdTrilha",
                table: "ModelUsuarioTrilhaProgresso",
                newName: "IX_ModelUsuarioTrilhaProgresso_IdTrilha");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ModelUsuarioTrilhaProgresso",
                table: "ModelUsuarioTrilhaProgresso",
                column: "IdProgresso");

            migrationBuilder.AddForeignKey(
                name: "FK_ModelUsuarioTrilhaProgresso_Trilhas_IdTrilha",
                table: "ModelUsuarioTrilhaProgresso",
                column: "IdTrilha",
                principalTable: "Trilhas",
                principalColumn: "IdTrilha",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ModelUsuarioTrilhaProgresso_Usuarios_IdUsuario",
                table: "ModelUsuarioTrilhaProgresso",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario",
                onDelete: ReferentialAction.Cascade);
        }
    }
}

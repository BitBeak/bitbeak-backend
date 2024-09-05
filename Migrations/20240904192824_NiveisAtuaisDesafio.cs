using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class NiveisAtuaisDesafio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NivelDesafiado",
                table: "Desafios",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NivelDesafiante",
                table: "Desafios",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NivelDesafiado",
                table: "Desafios");

            migrationBuilder.DropColumn(
                name: "NivelDesafiante",
                table: "Desafios");
        }
    }
}

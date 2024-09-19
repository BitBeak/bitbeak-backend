using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BitBeakAPI.Migrations
{
    /// <inheritdoc />
    public partial class NomeMissao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "Missoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "Missoes");
        }
    }
}

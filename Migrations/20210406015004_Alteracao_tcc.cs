using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaDeControleDeTCCs.Migrations
{
    public partial class Alteracao_tcc : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sigla",
                table: "Cursos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)");

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Cursos",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Sigla",
                table: "Cursos",
                type: "nvarchar(10)",
                nullable: false,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Nome",
                table: "Cursos",
                type: "nvarchar(30)",
                nullable: false,
                oldClrType: typeof(string));
        }
    }
}

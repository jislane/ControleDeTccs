using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaDeControleDeTCCs.Migrations
{
    public partial class relacionamentos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banca",
                columns: table => new
                {
                    BancaId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataDeCadastro = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banca", x => x.BancaId);
                });

            migrationBuilder.CreateTable(
                name: "Calendario",
                columns: table => new
                {
                    CalendarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DataApresentacao = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calendario", x => x.CalendarioId);
                });

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescStatus = table.Column<string>(type: "nvarchar(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusId);
                });

            migrationBuilder.CreateTable(
                name: "TipoUsuario",
                columns: table => new
                {
                    TipoUsuarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DescTipo = table.Column<string>(type: "nvarchar(250)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TipoUsuario", x => x.TipoUsuarioId);
                });

            migrationBuilder.CreateTable(
                name: "Usuario",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    Matricula = table.Column<string>(type: "nvarchar(25)", nullable: false),
                    cpf = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    telefone = table.Column<string>(type: "nvarchar(11)", nullable: true),
                    email = table.Column<string>(type: "nvarchar(25)", nullable: true),
                    TipoUsuarioId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.UsuarioId);
                    table.ForeignKey(
                        name: "FK_Usuario_TipoUsuario_TipoUsuarioId",
                        column: x => x.TipoUsuarioId,
                        principalTable: "TipoUsuario",
                        principalColumn: "TipoUsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Tccs",
                columns: table => new
                {
                    TccId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tema = table.Column<string>(type: "nvarchar(250)", nullable: false),
                    DataDeCadastro = table.Column<DateTime>(nullable: false),
                    StatusId = table.Column<int>(nullable: true),
                    BancaId = table.Column<int>(nullable: true),
                    UsuarioId = table.Column<int>(nullable: true),
                    CalendarioId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tccs", x => x.TccId);
                    table.ForeignKey(
                        name: "FK_Tccs_Banca_BancaId",
                        column: x => x.BancaId,
                        principalTable: "Banca",
                        principalColumn: "BancaId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tccs_Calendario_CalendarioId",
                        column: x => x.CalendarioId,
                        principalTable: "Calendario",
                        principalColumn: "CalendarioId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tccs_Status_StatusId",
                        column: x => x.StatusId,
                        principalTable: "Status",
                        principalColumn: "StatusId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tccs_Usuario_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuario",
                        principalColumn: "UsuarioId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_BancaId",
                table: "Tccs",
                column: "BancaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_CalendarioId",
                table: "Tccs",
                column: "CalendarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_StatusId",
                table: "Tccs",
                column: "StatusId");

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_UsuarioId",
                table: "Tccs",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Usuario_TipoUsuarioId",
                table: "Usuario",
                column: "TipoUsuarioId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tccs");

            migrationBuilder.DropTable(
                name: "Banca");

            migrationBuilder.DropTable(
                name: "Calendario");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Usuario");

            migrationBuilder.DropTable(
                name: "TipoUsuario");
        }
    }
}

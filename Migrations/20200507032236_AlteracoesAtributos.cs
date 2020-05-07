using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SistemaDeControleDeTCCs.Migrations
{
    public partial class AlteracoesAtributos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tccs_Banca_BancaId",
                table: "Tccs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tccs_Calendario_CalendarioId",
                table: "Tccs");

            migrationBuilder.DropIndex(
                name: "IX_Tccs_BancaId",
                table: "Tccs");

            migrationBuilder.DropIndex(
                name: "IX_Tccs_CalendarioId",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "BancaId",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "CalendarioId",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "DataApresentacao",
                table: "Calendario");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataApresentacao",
                table: "Tccs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFinalizacao",
                table: "Tccs",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LocalApresentacao",
                table: "Tccs",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Nota",
                table: "Tccs",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "Resumo",
                table: "Tccs",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Ano",
                table: "Calendario",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "Ativo",
                table: "Calendario",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataFim",
                table: "Calendario",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "DataInicio",
                table: "Calendario",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Semestre",
                table: "Calendario",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<double>(
                name: "Nota",
                table: "Banca",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "TccId",
                table: "Banca",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TipoUsuarioId",
                table: "Banca",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "Banca",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Banca_TccId",
                table: "Banca",
                column: "TccId");

            migrationBuilder.CreateIndex(
                name: "IX_Banca_TipoUsuarioId",
                table: "Banca",
                column: "TipoUsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_Banca_UsuarioId",
                table: "Banca",
                column: "UsuarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Banca_Tccs_TccId",
                table: "Banca",
                column: "TccId",
                principalTable: "Tccs",
                principalColumn: "TccId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Banca_TipoUsuario_TipoUsuarioId",
                table: "Banca",
                column: "TipoUsuarioId",
                principalTable: "TipoUsuario",
                principalColumn: "TipoUsuarioId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Banca_Usuario_UsuarioId",
                table: "Banca",
                column: "UsuarioId",
                principalTable: "Usuario",
                principalColumn: "UsuarioId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Banca_Tccs_TccId",
                table: "Banca");

            migrationBuilder.DropForeignKey(
                name: "FK_Banca_TipoUsuario_TipoUsuarioId",
                table: "Banca");

            migrationBuilder.DropForeignKey(
                name: "FK_Banca_Usuario_UsuarioId",
                table: "Banca");

            migrationBuilder.DropIndex(
                name: "IX_Banca_TccId",
                table: "Banca");

            migrationBuilder.DropIndex(
                name: "IX_Banca_TipoUsuarioId",
                table: "Banca");

            migrationBuilder.DropIndex(
                name: "IX_Banca_UsuarioId",
                table: "Banca");

            migrationBuilder.DropColumn(
                name: "DataApresentacao",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "DataFinalizacao",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "LocalApresentacao",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "Resumo",
                table: "Tccs");

            migrationBuilder.DropColumn(
                name: "Ano",
                table: "Calendario");

            migrationBuilder.DropColumn(
                name: "Ativo",
                table: "Calendario");

            migrationBuilder.DropColumn(
                name: "DataFim",
                table: "Calendario");

            migrationBuilder.DropColumn(
                name: "DataInicio",
                table: "Calendario");

            migrationBuilder.DropColumn(
                name: "Semestre",
                table: "Calendario");

            migrationBuilder.DropColumn(
                name: "Nota",
                table: "Banca");

            migrationBuilder.DropColumn(
                name: "TccId",
                table: "Banca");

            migrationBuilder.DropColumn(
                name: "TipoUsuarioId",
                table: "Banca");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "Banca");

            migrationBuilder.AddColumn<int>(
                name: "BancaId",
                table: "Tccs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CalendarioId",
                table: "Tccs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataApresentacao",
                table: "Calendario",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_BancaId",
                table: "Tccs",
                column: "BancaId");

            migrationBuilder.CreateIndex(
                name: "IX_Tccs_CalendarioId",
                table: "Tccs",
                column: "CalendarioId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tccs_Banca_BancaId",
                table: "Tccs",
                column: "BancaId",
                principalTable: "Banca",
                principalColumn: "BancaId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tccs_Calendario_CalendarioId",
                table: "Tccs",
                column: "CalendarioId",
                principalTable: "Calendario",
                principalColumn: "CalendarioId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}

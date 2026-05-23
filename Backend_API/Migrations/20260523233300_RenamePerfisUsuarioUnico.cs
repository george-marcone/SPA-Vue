using form_API.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace form_API.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20260523233300_RenamePerfisUsuarioUnico")]
    public partial class RenamePerfisUsuarioUnico : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 2,
                column: "DescricaoPerfil",
                value: "Professor");

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 3,
                column: "DescricaoPerfil",
                value: "Aluno");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 2,
                column: "DescricaoPerfil",
                value: "Contribuinte");

            migrationBuilder.UpdateData(
                table: "Perfil",
                keyColumn: "IdPerfil",
                keyValue: 3,
                column: "DescricaoPerfil",
                value: "Leitor");
        }
    }
}

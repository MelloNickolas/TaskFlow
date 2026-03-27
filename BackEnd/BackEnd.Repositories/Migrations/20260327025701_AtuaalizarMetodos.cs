using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackEnd.Repositories.Migrations
{
    public partial class AtuaalizarMetodos : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataAtribuicao",
                table: "UsuariosTarefas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(2026, 3, 26),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValue: new DateOnly(2026, 3, 15));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "DataAtribuicao",
                table: "UsuariosTarefas",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateOnly(2026, 3, 15),
                oldClrType: typeof(DateOnly),
                oldType: "TEXT",
                oldDefaultValue: new DateOnly(2026, 3, 26));
        }
    }
}

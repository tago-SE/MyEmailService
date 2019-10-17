using Microsoft.EntityFrameworkCore.Migrations;

namespace MyEmailService.Data.Migrations
{
    public partial class Refresh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessegeState",
                table: "Messeges");

            migrationBuilder.AddColumn<int>(
                name: "MessageState",
                table: "Messeges",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MessageState",
                table: "Messeges");

            migrationBuilder.AddColumn<int>(
                name: "MessegeState",
                table: "Messeges",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

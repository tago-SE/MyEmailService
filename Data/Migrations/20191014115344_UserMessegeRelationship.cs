using Microsoft.EntityFrameworkCore.Migrations;

namespace MyEmailService.Data.Migrations
{
    public partial class UserMessegeRelationship : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ReceiverId",
                table: "Messeges",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderId",
                table: "Messeges",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Messeges_ReceiverId",
                table: "Messeges",
                column: "ReceiverId");

            migrationBuilder.CreateIndex(
                name: "IX_Messeges_SenderId",
                table: "Messeges",
                column: "SenderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messeges_AspNetUsers_ReceiverId",
                table: "Messeges",
                column: "ReceiverId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messeges_AspNetUsers_SenderId",
                table: "Messeges",
                column: "SenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Messeges_AspNetUsers_ReceiverId",
                table: "Messeges");

            migrationBuilder.DropForeignKey(
                name: "FK_Messeges_AspNetUsers_SenderId",
                table: "Messeges");

            migrationBuilder.DropIndex(
                name: "IX_Messeges_ReceiverId",
                table: "Messeges");

            migrationBuilder.DropIndex(
                name: "IX_Messeges_SenderId",
                table: "Messeges");

            migrationBuilder.DropColumn(
                name: "ReceiverId",
                table: "Messeges");

            migrationBuilder.DropColumn(
                name: "SenderId",
                table: "Messeges");
        }
    }
}

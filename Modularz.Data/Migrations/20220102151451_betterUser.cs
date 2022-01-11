using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modularz.Data.Migrations
{
    public partial class betterUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "BlogUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HashedPassword",
                table: "BlogUsers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Salt",
                table: "BlogUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "BlogUsers");

            migrationBuilder.DropColumn(
                name: "HashedPassword",
                table: "BlogUsers");

            migrationBuilder.DropColumn(
                name: "Salt",
                table: "BlogUsers");
        }
    }
}

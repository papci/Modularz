using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Modularz.Data.Migrations
{
    public partial class tempTokoen : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TempToken",
                table: "BlogUsers",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TempToken",
                table: "BlogUsers");
        }
    }
}

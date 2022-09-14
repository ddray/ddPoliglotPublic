using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class addMetaKeys : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PageMetaDescription",
                table: "Lessons",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PageMetaTitle",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PageMetaDescription",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "PageMetaTitle",
                table: "Lessons");
        }
    }
}

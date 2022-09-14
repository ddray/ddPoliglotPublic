using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class lessonimages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image1",
                table: "Lessons",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image2",
                table: "Lessons",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image3",
                table: "Lessons",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image4",
                table: "Lessons",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image5",
                table: "Lessons",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image1",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Image2",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Image3",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Image4",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Image5",
                table: "Lessons");
        }
    }
}

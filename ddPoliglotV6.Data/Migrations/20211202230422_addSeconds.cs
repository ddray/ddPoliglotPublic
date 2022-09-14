using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class addSeconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LearnSecundes",
                table: "UserLessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalSecundes",
                table: "UserLessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnSecundes",
                table: "UserLessons");

            migrationBuilder.DropColumn(
                name: "TotalSecundes",
                table: "UserLessons");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class deleteMinutes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnMinutes",
                table: "UserLessons");

            migrationBuilder.DropColumn(
                name: "TotalMinutes",
                table: "UserLessons");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LearnMinutes",
                table: "UserLessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMinutes",
                table: "UserLessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}

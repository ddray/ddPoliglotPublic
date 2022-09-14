using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class lessonType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TotalSecundes",
                table: "UserLessons",
                newName: "TotalSeconds");

            migrationBuilder.RenameColumn(
                name: "LearnSecundes",
                table: "UserLessons",
                newName: "LessonType");

            migrationBuilder.AddColumn<int>(
                name: "LearnSeconds",
                table: "UserLessons",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LearnSeconds",
                table: "UserLessons");

            migrationBuilder.RenameColumn(
                name: "TotalSeconds",
                table: "UserLessons",
                newName: "TotalSecundes");

            migrationBuilder.RenameColumn(
                name: "LessonType",
                table: "UserLessons",
                newName: "LearnSecundes");
        }
    }
}

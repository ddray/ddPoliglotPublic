using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class word_user_word_type : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "wordType",
                table: "UserLessonWords",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "wordType",
                table: "UserLessonWords");
        }
    }
}

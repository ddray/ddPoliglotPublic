using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class addFieldsForRep : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LastRepeatInArticleByParamID",
                table: "WordUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastRepeatInLessonNum",
                table: "WordUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRepeatInArticleByParamID",
                table: "WordUsers");

            migrationBuilder.DropColumn(
                name: "LastRepeatInLessonNum",
                table: "WordUsers");
        }
    }
}

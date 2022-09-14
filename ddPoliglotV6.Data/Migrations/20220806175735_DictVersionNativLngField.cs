using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class DictVersionNativLngField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "NativeLanguageID",
                table: "DictionaryVersions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NativeLanguageID",
                table: "DictionaryVersions");
        }
    }
}

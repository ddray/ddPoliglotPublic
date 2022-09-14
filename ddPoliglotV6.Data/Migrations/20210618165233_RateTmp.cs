using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class RateTmp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OxfordLevel",
                table: "Words",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RateTmp",
                table: "Words",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OxfordLevel",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "RateTmp",
                table: "Words");
        }
    }
}

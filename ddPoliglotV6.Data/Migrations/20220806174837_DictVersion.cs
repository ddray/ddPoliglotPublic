using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class DictVersion : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DictionaryVersions",
                columns: table => new
                {
                    DictionaryVersionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Value = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DictionaryVersions", x => x.DictionaryVersionID);
                    table.ForeignKey(
                        name: "FK_DictionaryVersions_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DictionaryVersions_LanguageID",
                table: "DictionaryVersions",
                column: "LanguageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DictionaryVersions");
        }
    }
}

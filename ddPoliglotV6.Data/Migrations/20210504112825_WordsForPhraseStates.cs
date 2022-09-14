using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class WordsForPhraseStates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WordsForPhraseStates",
                columns: table => new
                {
                    WordsForPhraseStateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    NativeLanguageID = table.Column<int>(type: "int", nullable: false),
                    WordsForPhraseStateData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordsForPhraseStates", x => x.WordsForPhraseStateID);
                    table.ForeignKey(
                        name: "FK_WordsForPhraseStates_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WordsForPhraseStates_LanguageID",
                table: "WordsForPhraseStates",
                column: "LanguageID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WordsForPhraseStates");
        }
    }
}

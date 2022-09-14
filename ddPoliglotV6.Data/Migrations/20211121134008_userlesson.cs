using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class userlesson : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserLessons",
                columns: table => new
                {
                    UserLessonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Num = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLessons", x => x.UserLessonID);
                });

            migrationBuilder.CreateTable(
                name: "UserLessonWords",
                columns: table => new
                {
                    UserLessonWordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserLessonID = table.Column<int>(type: "int", nullable: false),
                    WordID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLessonWords", x => x.UserLessonWordID);
                    table.ForeignKey(
                        name: "FK_UserLessonWords_UserLessons_UserLessonID",
                        column: x => x.UserLessonID,
                        principalTable: "UserLessons",
                        principalColumn: "UserLessonID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserLessonWords_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLessonWords_UserLessonID",
                table: "UserLessonWords",
                column: "UserLessonID");

            migrationBuilder.CreateIndex(
                name: "IX_UserLessonWords_WordID",
                table: "UserLessonWords",
                column: "WordID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLessonWords");

            migrationBuilder.DropTable(
                name: "UserLessons");
        }
    }
}

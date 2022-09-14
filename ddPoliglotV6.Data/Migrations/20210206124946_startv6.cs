using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ddPoliglotV6.Data.Migrations
{
    public partial class startv6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ArticleByParams",
                columns: table => new
                {
                    ArticleByParamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NativeLanguageID = table.Column<int>(type: "int", nullable: false),
                    LearnLanguageID = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    DataJson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsTemplate = table.Column<bool>(type: "bit", nullable: false),
                    IsShared = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleByParams", x => x.ArticleByParamID);
                });

            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    ArticleID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Language = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LanguageTranslation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextHashCode = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    VideoFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.ArticleID);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsSuperAdmin = table.Column<bool>(type: "bit", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Languages",
                columns: table => new
                {
                    LanguageID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Code = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CodeFull = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    ShortName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.LanguageID);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogID);
                });

            migrationBuilder.CreateTable(
                name: "MixParams",
                columns: table => new
                {
                    MixParamID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ArticlePhraseKeyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TextHashCode = table.Column<long>(type: "bigint", nullable: false),
                    TrTextHashCode = table.Column<long>(type: "bigint", nullable: false),
                    TrFirst = table.Column<bool>(type: "bit", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    M0 = table.Column<bool>(type: "bit", nullable: false),
                    M0_repeat = table.Column<bool>(type: "bit", nullable: false),
                    M01 = table.Column<bool>(type: "bit", nullable: false),
                    M01_repeat = table.Column<bool>(type: "bit", nullable: false),
                    M012 = table.Column<bool>(type: "bit", nullable: false),
                    M012_repeat = table.Column<bool>(type: "bit", nullable: false),
                    M0123 = table.Column<bool>(type: "bit", nullable: false),
                    M0123_repeat = table.Column<bool>(type: "bit", nullable: false),
                    TrActive = table.Column<bool>(type: "bit", nullable: false),
                    TrM0 = table.Column<bool>(type: "bit", nullable: false),
                    TrM0_repeat = table.Column<bool>(type: "bit", nullable: false),
                    TrM01 = table.Column<bool>(type: "bit", nullable: false),
                    TrM01_repeat = table.Column<bool>(type: "bit", nullable: false),
                    TrM012 = table.Column<bool>(type: "bit", nullable: false),
                    TrM012_repeat = table.Column<bool>(type: "bit", nullable: false),
                    TrM0123 = table.Column<bool>(type: "bit", nullable: false),
                    TrM0123_repeat = table.Column<bool>(type: "bit", nullable: false),
                    Offer1 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Offer2 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Offer3 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Offer4 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Offer5 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrOffer1 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrOffer2 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrOffer3 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrOffer4 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TrOffer5 = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    PhrasesMixType = table.Column<int>(type: "int", nullable: false),
                    Repeat = table.Column<int>(type: "int", nullable: false),
                    TrRepeat = table.Column<int>(type: "int", nullable: false),
                    RepeatOrder = table.Column<int>(type: "int", nullable: false),
                    TrRepeatOrder = table.Column<int>(type: "int", nullable: false),
                    addSlowInRepeatOrder = table.Column<bool>(type: "bit", nullable: false),
                    addSlow2InRepeatOrder = table.Column<bool>(type: "bit", nullable: false),
                    TrAddSlowInRepeatOrder = table.Column<bool>(type: "bit", nullable: false),
                    TrAddSlow2InRepeatOrder = table.Column<bool>(type: "bit", nullable: false),
                    RepeatBaseWord = table.Column<int>(type: "int", nullable: false),
                    TrRepeatBaseWord = table.Column<int>(type: "int", nullable: false),
                    firstDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    firstBeforeDialogDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeByOrderMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideByOrderMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeBaseWordsDirMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideBaseWordsDirMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeBaseWordsRevMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideBaseWordsRevMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeAllDirMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideAllDirMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeAllRevMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    insideAllRevMixDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    beforeFinishDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    finishDictorPhrases = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixParams", x => x.MixParamID);
                });

            migrationBuilder.CreateTable(
                name: "MixParamTextTemps",
                columns: table => new
                {
                    MixParamTextTempID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    KeyTemp = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LearnLanguageID = table.Column<int>(type: "int", nullable: false),
                    NativeLanguageID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixParamTextTemps", x => x.MixParamTextTempID);
                });

            migrationBuilder.CreateTable(
                name: "SpeechFiles",
                columns: table => new
                {
                    SpeechFileID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    SpeechFileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpeechFiles", x => x.SpeechFileID);
                });

            migrationBuilder.CreateTable(
                name: "ArticleActors",
                columns: table => new
                {
                    ArticleActorID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    DefaultInRole = table.Column<bool>(type: "bit", nullable: false),
                    Language = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    VoiceName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    VoiceSpeed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    VoicePitch = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ArticleID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleActors", x => x.ArticleActorID);
                    table.ForeignKey(
                        name: "FK_ArticleActors_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ArticleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    LessonID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    NativeLanguageID = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Title = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<int>(type: "int", nullable: false),
                    Video1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Video2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Video3 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Video4 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Video5 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Audio1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Audio2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Audio3 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Audio4 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Audio5 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Description1 = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Description2 = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Description3 = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Description4 = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Description5 = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true),
                    Modified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    State = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    PageName = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    ArticleByParamID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.LessonID);
                    table.ForeignKey(
                        name: "FK_Lessons_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WordPhrases",
                columns: table => new
                {
                    WordPhraseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(350)", maxLength: 350, nullable: true),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDuration = table.Column<int>(type: "int", nullable: false),
                    HashCodeSpeed1 = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileNameSpeed1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDurationSpeed1 = table.Column<int>(type: "int", nullable: false),
                    HashCodeSpeed2 = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileNameSpeed2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDurationSpeed2 = table.Column<int>(type: "int", nullable: false),
                    SourceType = table.Column<int>(type: "int", nullable: false),
                    WordsUsed = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPhrases", x => x.WordPhraseID);
                    table.ForeignKey(
                        name: "FK_WordPhrases_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Words",
                columns: table => new
                {
                    WordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    Rate = table.Column<int>(type: "int", nullable: false),
                    Pronunciation = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDuration = table.Column<int>(type: "int", nullable: false),
                    HashCodeSpeed1 = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileNameSpeed1 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDurationSpeed1 = table.Column<int>(type: "int", nullable: false),
                    HashCodeSpeed2 = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileNameSpeed2 = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDurationSpeed2 = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Words", x => x.WordID);
                    table.ForeignKey(
                        name: "FK_Words_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MixItems",
                columns: table => new
                {
                    MixItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MixParamID = table.Column<int>(type: "int", nullable: false),
                    KeyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Source = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    InDict = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    InContext = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ChildrenType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    EndPhrase = table.Column<bool>(type: "bit", nullable: false),
                    Pretext = table.Column<bool>(type: "bit", nullable: false),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    baseWord = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MixItems", x => x.MixItemID);
                    table.ForeignKey(
                        name: "FK_MixItems_MixParams_MixParamID",
                        column: x => x.MixParamID,
                        principalTable: "MixParams",
                        principalColumn: "MixParamID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ArticlePhrases",
                columns: table => new
                {
                    ArticlePhraseID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KeyGuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ArticleID = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDuration = table.Column<int>(type: "int", nullable: false),
                    Pause = table.Column<int>(type: "int", nullable: false),
                    ArticleActorID = table.Column<int>(type: "int", nullable: false),
                    TrText = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    TrHashCode = table.Column<long>(type: "bigint", nullable: false),
                    TrTextSpeechFileName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    TrSpeachDuration = table.Column<int>(type: "int", nullable: false),
                    TrPause = table.Column<int>(type: "int", nullable: false),
                    TrArticleActorID = table.Column<int>(type: "int", nullable: false),
                    OrderNum = table.Column<int>(type: "int", nullable: false),
                    ActivityType = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    ParentKeyGuid = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ChildrenType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    HasChildren = table.Column<bool>(type: "bit", nullable: false),
                    ChildrenClosed = table.Column<bool>(type: "bit", nullable: false),
                    Silent = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticlePhrases", x => x.ArticlePhraseID);
                    table.ForeignKey(
                        name: "FK_ArticlePhrases_ArticleActors_ArticleActorID",
                        column: x => x.ArticleActorID,
                        principalTable: "ArticleActors",
                        principalColumn: "ArticleActorID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArticlePhrases_ArticleActors_TrArticleActorID",
                        column: x => x.TrArticleActorID,
                        principalTable: "ArticleActors",
                        principalColumn: "ArticleActorID",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_ArticlePhrases_Articles_ArticleID",
                        column: x => x.ArticleID,
                        principalTable: "Articles",
                        principalColumn: "ArticleID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WordPhraseTranslations",
                columns: table => new
                {
                    WordPhraseTranslationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordPhraseID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDuration = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPhraseTranslations", x => x.WordPhraseTranslationID);
                    table.ForeignKey(
                        name: "FK_WordPhraseTranslations_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordPhraseTranslations_WordPhrases_WordPhraseID",
                        column: x => x.WordPhraseID,
                        principalTable: "WordPhrases",
                        principalColumn: "WordPhraseID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WordPhraseWords",
                columns: table => new
                {
                    WordPhraseWordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordID = table.Column<int>(type: "int", nullable: false),
                    WordPhraseID = table.Column<int>(type: "int", nullable: false),
                    PhraseOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordPhraseWords", x => x.WordPhraseWordID);
                    table.ForeignKey(
                        name: "FK_WordPhraseWords_WordPhrases_WordPhraseID",
                        column: x => x.WordPhraseID,
                        principalTable: "WordPhrases",
                        principalColumn: "WordPhraseID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordPhraseWords_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WordTranslations",
                columns: table => new
                {
                    WordTranslationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordID = table.Column<int>(type: "int", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: true),
                    HashCode = table.Column<long>(type: "bigint", nullable: false),
                    TextSpeechFileName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SpeachDuration = table.Column<int>(type: "int", nullable: false),
                    ReadyForLessonPhrasiesCnt = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordTranslations", x => x.WordTranslationID);
                    table.ForeignKey(
                        name: "FK_WordTranslations_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordTranslations_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "WordUsers",
                columns: table => new
                {
                    WordUserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WordID = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageID = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WordUsers", x => x.WordUserID);
                    table.ForeignKey(
                        name: "FK_WordUsers_Languages_LanguageID",
                        column: x => x.LanguageID,
                        principalTable: "Languages",
                        principalColumn: "LanguageID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WordUsers_Words_WordID",
                        column: x => x.WordID,
                        principalTable: "Words",
                        principalColumn: "WordID",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ArticleActors_ArticleID",
                table: "ArticleActors",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePhrases_ArticleActorID",
                table: "ArticlePhrases",
                column: "ArticleActorID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePhrases_ArticleID",
                table: "ArticlePhrases",
                column: "ArticleID");

            migrationBuilder.CreateIndex(
                name: "IX_ArticlePhrases_TrArticleActorID",
                table: "ArticlePhrases",
                column: "TrArticleActorID");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_LanguageID",
                table: "Lessons",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_MixItems_MixParamID",
                table: "MixItems",
                column: "MixParamID");

            migrationBuilder.CreateIndex(
                name: "IX_WordPhrases_LanguageID",
                table: "WordPhrases",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_WordPhraseTranslations_LanguageID",
                table: "WordPhraseTranslations",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_WordPhraseTranslations_WordPhraseID",
                table: "WordPhraseTranslations",
                column: "WordPhraseID");

            migrationBuilder.CreateIndex(
                name: "IX_WordPhraseWords_WordID",
                table: "WordPhraseWords",
                column: "WordID");

            migrationBuilder.CreateIndex(
                name: "IX_WordPhraseWords_WordPhraseID",
                table: "WordPhraseWords",
                column: "WordPhraseID");

            migrationBuilder.CreateIndex(
                name: "IX_Words_LanguageID",
                table: "Words",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslations_LanguageID",
                table: "WordTranslations",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_WordTranslations_WordID",
                table: "WordTranslations",
                column: "WordID");

            migrationBuilder.CreateIndex(
                name: "IX_WordUsers_LanguageID",
                table: "WordUsers",
                column: "LanguageID");

            migrationBuilder.CreateIndex(
                name: "IX_WordUsers_WordID",
                table: "WordUsers",
                column: "WordID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleByParams");

            migrationBuilder.DropTable(
                name: "ArticlePhrases");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "MixItems");

            migrationBuilder.DropTable(
                name: "MixParamTextTemps");

            migrationBuilder.DropTable(
                name: "SpeechFiles");

            migrationBuilder.DropTable(
                name: "WordPhraseTranslations");

            migrationBuilder.DropTable(
                name: "WordPhraseWords");

            migrationBuilder.DropTable(
                name: "WordTranslations");

            migrationBuilder.DropTable(
                name: "WordUsers");

            migrationBuilder.DropTable(
                name: "ArticleActors");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "MixParams");

            migrationBuilder.DropTable(
                name: "WordPhrases");

            migrationBuilder.DropTable(
                name: "Words");

            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "Languages");
        }
    }
}

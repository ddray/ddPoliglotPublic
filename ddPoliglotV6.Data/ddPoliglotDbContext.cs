using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ddPoliglotV6.Data
{
    public class ddPoliglotDbContext : IdentityDbContext<ApplicationUser>
    {
        IConfiguration _configuration;
        
        public ddPoliglotDbContext(IConfiguration configuration) : base(new DbContextOptionsBuilder<ddPoliglotDbContext>().Options)
        {
            _configuration = configuration;
        }

        public ddPoliglotDbContext(IConfiguration configuration,
            DbContextOptions options) : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
            //optionsBuilder.UseSqlServer(connectionString, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "Sa_123456"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.HasDefaultSchema("Sa_123456");
            //foreach (var et in modelBuilder.Model.GetEntityTypes())
            //{
            //    et.SetSchema("Sa_123456");
            //}
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Article> Articles { get; set; }
        public DbSet<ArticlePhrase> ArticlePhrases { get; set; }
        public DbSet<SpeechFile> SpeechFiles { get; set; }
        public DbSet<ArticleActor> ArticleActors { get; set; }
        public DbSet<MixItem> MixItems { get; set; }
        public DbSet<MixParam> MixParams { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Language> Languages { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordPhrase> WordPhrases { get; set; }
        public DbSet<WordPhraseWord> WordPhraseWords { get; set; }
        public DbSet<WordUser> WordUsers { get; set; }
        public DbSet<WordTranslation> WordTranslations { get; set; }
        public DbSet<WordPhraseTranslation> WordPhraseTranslations { get; set; }
        public DbSet<MixParamTextTemp> MixParamTextTemps { get; set; }
        public DbSet<ArticleByParam> ArticleByParams { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<WordsForPhraseState> WordsForPhraseStates { get; set; }
        public DbSet<UserLanguageLevel> UserLanguageLevels { get; set; }
        public DbSet<UserLesson> UserLessons { get; set; }
        public DbSet<UserLessonWord> UserLessonWords { get; set; }
        public DbSet<DictionaryVersion> DictionaryVersions { get; set; }
        
    }
}

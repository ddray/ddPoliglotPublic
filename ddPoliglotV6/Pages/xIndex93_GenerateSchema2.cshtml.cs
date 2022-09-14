using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Helpers;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using EFCore.BulkExtensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ddPoliglotV6.Pages
{
    public class xIndex93_GenerateSchema2Model : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;

        public xIndex93_GenerateSchema2Model(
            ddPoliglotDbContext context,
            ILogger<IndexModel> logger,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            UserManager = userManager;
        }

        public List<Lesson> Lessons = new List<Lesson>();
        public List<Word> Words = new List<Word>();
        private readonly static int WordsInLessonQty = 5;
        private readonly List<int> SchemaOfWordRepInWordPhrases = new List<int>() { 8, 16, 32, 64, 128, 256, 512, 1024 };
        private readonly static int wordRequiredRepetitionInLessonsQty = 3;
        private readonly static int maxWordsForRepetition = WordsInLessonQty * 5;

        public async Task OnGetAsync()
        {
            GenerateSchema();
        }

        private void GenerateSchema()
        {
            GenerateWords();

            for (int lessonNum = 1; lessonNum < 1000; lessonNum++)
            {
                AddLesson(lessonNum);
            }
        }

        private void AddLesson(int lessonNum)
        {
            var firstWord = (lessonNum - 1) * WordsInLessonQty;

            var lesson = new Lesson()
            {
                Num = lessonNum,
                Words = new List<Word>(),
                WordsForRepetitionInPhrases = new List<Word>(),
                 WordsWithPhrasesForRepetition = new List<Word>()
            };

            for (int index = 0; index < WordsInLessonQty; index++)
            {
                var num = firstWord + index;
                if (num < this.Words.Count())
                {
                    lesson.Words.Add(this.Words[num]);
                    lesson.WordsForRepetitionInPhrases.AddRange(this.Words[num].NeedUseInPhrases);
                }
            }

            // lesson.WordsWithPhrasesForRepetition = GetListForRepetition(lessonNum);

            // mark words as learned
            foreach (var word in lesson.Words)
            {
                word.WordUser.Grade = 1;
            }

            this.Lessons.Add(lesson);
        }

        private void GenerateWords()
        {
            var result = new List<Word>();

            for (int i = 0; i < 5000; i++)
            {
                var word = new Word()
                {
                    WordID = i + 1,
                    Rate = i + 1,
                    Text = $"w{i+1}. ",
                    WordPhraseSelected = new List<WordPhrase>(),
                    NeedUseInPhrases = new List<Word>(),
                    WordUser = new WordUser() {  Grade = 0}
                };

                // add phrases to this word
                for (int indexPhrase = 0; indexPhrase < 3; indexPhrase++)
                {
                    var wordPhrase = new WordPhrase()
                    {
                        Text = $"ph-{indexPhrase}_{word.Text}",
                        WordPhraseID = indexPhrase + 1,
                    };

                    word.WordPhraseSelected.Add(wordPhrase);
                }

                // add words that are needed to use in phrases
                foreach (var val in this.SchemaOfWordRepInWordPhrases)
                {
                    var wordIndex = val * WordsInLessonQty;
                    var index = i - wordIndex;
                    if (index >= 0)
                    {
                        word.NeedUseInPhrases.Add(result[index]);
                    }
                }

                result.Add(word);
            }

            this.Words = result;
        }

        public class Lesson
        { 
            public int Num { get; set; }
            public List<Word> Words { get; set; }
            public List<Word> WordsForRepetitionInPhrases { get; set; }
            public List<Word> WordsWithPhrasesForRepetition { get; set; }
        }
    }
}

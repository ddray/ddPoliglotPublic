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
    public class xIndex92_GenerateSchemaModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;

        public xIndex92_GenerateSchemaModel(
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

            lesson.WordsWithPhrasesForRepetition = GetListForRepetition(lessonNum);

            // mark words as learned
            foreach (var word in lesson.Words)
            {
                word.WordUser.Grade = 1;
            }

            this.Lessons.Add(lesson);
        }

        public List<Word> GetListForRepetition(int parentID)
        {
            var wordsInLesson = 5;
            var wordRequiredRepetitionInLessonsQty = 3;
            var maxWordsForRepetition = wordsInLesson * 5;
            var result = new List<Word>();

            #region get elder lessons with same base name

            var oldArticlesByParams = this.Lessons.Where(x => parentID == 0 || x.Num < parentID).ToList();

            #endregion get elder lessons with same base name

            #region get words for repetition from 3 latest lessons

            var latestArticlesByParams = oldArticlesByParams
                .OrderByDescending(x => x.Num)
                .Take(wordRequiredRepetitionInLessonsQty)
                .ToList();

            var latestArticlesByParamsIndex = 0;
            foreach (var articlesByParams in latestArticlesByParams)
            {
                foreach (var word in articlesByParams.Words)
                {
                    var wordForRep = new Word()
                    {
                        Text = word.Text,
                        WordID = word.WordID
                    };

                    result.Add(wordForRep);

                    // get phrase
                    var index = latestArticlesByParamsIndex < word.WordPhraseSelected.Count()
                        ? latestArticlesByParamsIndex
                        : 0;

                    wordForRep.WordPhraseSelected = new List<WordPhrase>();
                    wordForRep.WordPhraseSelected.Add(word.WordPhraseSelected[index]);
                }

                latestArticlesByParamsIndex++;
            }

            #endregion get words for repetition from 3 latest lessons

            #region get current lesson number by order from previous lessons

            var curLessonNumber = parentID;

            #endregion get current lesson number by order from previous lessons

            #region get words for repetition by grade and history

            // get words candidates to local list
            var words = this.Words
                .Where(x => x.WordUser != null && x.WordUser.Grade > 0 && x.WordUser.Grade < 5)
                .OrderByDescending(x => (5 - x.WordUser.Grade) * (curLessonNumber - x.WordUser.LastRepeatInLessonNum))
                .ThenBy(x => x.Rate)
                .Take(maxWordsForRepetition + result.Count())
                .ToList();

            foreach (var word in words)
            {
                if (!result.Any(x => x.WordID == word.WordID))
                {
                    if (result.Count() < maxWordsForRepetition)
                    {
                        var wordForRep = new Word()
                        {
                            Text = word.Text,
                            WordID = word.WordID
                        };

                        // get phrase for this word
                        //word.WordPhraseSelected = word.WordPhraseSelected
                        //    .Where(x => x.PhraseOrderInCurrentWord > 0)
                        //    .Where(x => x.WordPhraseTranslation != null)
                        //    .ToList();

                        var phrase = GetPhraseForRepetition(word);
                        wordForRep.WordPhraseSelected = new List<WordPhrase>();
                        if (phrase != null)
                        {
                            wordForRep.WordPhraseSelected.Add(phrase);
                            word.WordUser.LastRepeatWordPhraseId = phrase.WordPhraseID;
                        }

                        result.Add(wordForRep);

                        // mark
                        word.WordUser.LastRepeatInLessonNum = parentID;
                    }
                }
            }

            #endregion get words for repetition by grade and history

            return result;
        }

        private WordPhrase GetPhraseForRepetition(Word word)
        {
            if (word.WordPhraseSelected.Count() == 0)
            {
                return null;
            }

            if (word.WordUser.LastRepeatWordPhraseId == 0)
            {
                return word.WordPhraseSelected[0];
            }

            var index = 0;
            foreach (var phrase in word.WordPhraseSelected)
            {
                if (phrase.WordPhraseID == word.WordUser.LastRepeatWordPhraseId)
                {
                    break;
                }

                index++;
            }

            if (index >= (word.WordPhraseSelected.Count() - 1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            return word.WordPhraseSelected[index];
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
                var currPhraseIndex = 0;
                foreach (var val in this.SchemaOfWordRepInWordPhrases)
                {
                    var wordIndex = val * WordsInLessonQty;
                    var index = i - wordIndex;
                    if (index >= 0)
                    {
                        word.NeedUseInPhrases.Add(result[index]);

                        // add to current phrase word for mentions
                        word.WordPhraseSelected[currPhraseIndex].Text += $" {result[index].Text},";
                        currPhraseIndex++;
                        if (currPhraseIndex > 2)
                        {
                            currPhraseIndex = 0;
                        }
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

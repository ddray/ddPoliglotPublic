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
    public class xIndex91_RepeatWordsInLessonsModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;

        public xIndex91_RepeatWordsInLessonsModel(
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

        public List<testLesson> Lessons = new List<testLesson>();
        public List<testWord> Words = new List<testWord>();

        public async Task OnGetAsync()
        {
            // TestGetRepetition();
        }

        async private Task UpdatePhrasesOrdersByUsingInArticlesByParams()
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            var query = _context.ArticleByParams.AsNoTracking()
                .Where(x => x.LearnLanguageID == (int)Languages.en
                && x.NativeLanguageID == (int)Languages.ru
                && (x.UserID == userId || x.IsShared)
                && x.Type == 0
                )
                .Select(x => x).OrderBy(x=>x.ArticleByParamID);

            var result = await query.ToListAsync();
            foreach (var item in result)
            {
                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(item.DataJson);
                // mark order of phrases used in this lesson
                if (articleByParamData.SelectedWords != null)
                {
                    foreach (var selectedWord in articleByParamData.SelectedWords)
                    {
                        var index = 0;
                        foreach (var phrase in selectedWord.PhraseWordsSelected)
                        {
                            var wordPhraseWords = _context.WordPhraseWords
                                .FirstOrDefault(x =>
                                    x.WordID == selectedWord.Word.WordID
                                    && x.WordPhraseID == phrase.WordPhraseID
                                );

                            if (wordPhraseWords != null)
                            {
                                wordPhraseWords.PhraseOrder = index + 1;
                                index++;

                                _context.Update(wordPhraseWords);
                                _context.SaveChanges();
                            }
                            else
                            {
                                Debug.Print($"error: word:{selectedWord.Word.Text}/{selectedWord.Word.WordID} --- phrase: {phrase.Text}/{phrase.WordPhraseID}");
                            }
                        }
                    }
                }

            }
        }

        #region test calc repetition list

        public void TestGetRepetition()
        {
            var wordsInLesson = 2;
            var wordRequiredRepetitionInLessonsQty = 3;
            var maxWordsForRepetition = wordsInLesson * 4;
            // this.Words = InitTestGetRepetitionData(20);
            this.Words = InitWordsList(20);
            var lessonsMax = 10;
            var lessonIndex = 0;

            for (; ;)
            {
                if (++lessonIndex > lessonsMax)
                {
                    break;
                }

                // get 
                // var curLessonNumber = this.Lessons.Count() + 1;

                var curLessonNumber = this.Words.Max(x=>x.LastRepeat) + 1;

                var lessonItem = new testLesson
                {
                    Num = curLessonNumber,
                    WordsInLesson = new List<testWord>(),

                    WordsForRepetition = new List<testWord>(),
                    PhrasesForRepetition = new List<testPhrase>()
                };

                // get words for lesson
                // words by Rate without grade 
                lessonItem.WordsInLesson = this.Words
                    .Where(x => x.Grade == 0)
                    .OrderBy(x => x.Rate)
                    .Take(wordsInLesson)
                    .ToList();

                // get words for repetition from 3 latest lessons
                var latestLessons = this.Lessons
                    .OrderByDescending(x => x.Num)
                    .Take(wordRequiredRepetitionInLessonsQty)
                    .ToList();

                foreach (var lesson in latestLessons)
                {
                    foreach (var word in lesson.WordsInLesson)
                    {
                        lessonItem.WordsForRepetition.Add(word);

                        var phrase = GetPhraseForRepetition(word);
                        lessonItem.PhrasesForRepetition.Add(phrase);
                        word.LastPhraseRepeatId = phrase.Id;
                    }
                }

                // get words from repetition by grade and history
                var wordsForRepetitionByGrade = this.Words
                    .Where(x => x.Grade > 0)
                    .OrderByDescending(x => (5 - x.Grade) * (curLessonNumber - x.LastRepeat))
                    .ThenBy(x => x.Rate)
                    .Take(maxWordsForRepetition + lessonItem.WordsForRepetition.Count())
                    .ToList();

                foreach (var word in wordsForRepetitionByGrade)
                {
                    if (!lessonItem.WordsForRepetition.Any(x => x.Text == word.Text))
                    {
                        if (lessonItem.WordsForRepetition.Count() < maxWordsForRepetition)
                        {
                            word.LastRepeat = curLessonNumber;
                            word.QtyRepeat++;
                            lessonItem.WordsForRepetition.Add(word);

                            var phrase = GetPhraseForRepetition(word);
                            lessonItem.PhrasesForRepetition.Add(phrase);
                            word.LastPhraseRepeatId = phrase.Id;
                        }
                    }
                }

                // mark lessons as started to learn
                lessonItem.WordsInLesson.ForEach(x => x.Grade = 1);

                // lessonItem.CurrState = string.Join(',', lessonData.OrderBy(x => x.Rate).Select(x => x.CurrWeight.ToString())); 

                Lessons.Add(lessonItem);
            }
        }

        private List<testWord> InitWordsList(int count)
        {
            var result = new List<testWord>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new testWord
                {
                    Rate = i+1,
                    Text = $"word_{i+1}",
                    Grade = 0
                });
            }

            foreach (var word in result)
            {
                word.Phrases = new List<testPhrase>();
                for (int i = 0; i < 3; i++)
                {
                    word.Phrases.Add(new testPhrase
                    {
                        Order = i + 1,
                        Text = $"phrase {i + 1} ({word.Text})",
                        Id = i + 1
                    });
                }
            }

            return result;
        }

        private testPhrase GetPhraseForRepetition(testWord word)
        {
            if (word.Phrases.Count() == 0)
            {
                return null;
            }

            if (word.LastPhraseRepeatId == 0)
            {
                return word.Phrases[0];
            }

            var index = 0;
            foreach (var phrase in word.Phrases)
            {
                if (phrase.Id == word.LastPhraseRepeatId)
                {
                    break;
                }

                index++;
            }

            if (index >= (word.Phrases.Count() - 1))
            {
                index = 0;
            }
            else
            {
                index++;
            }

            return word.Phrases[index];
        }

        private List<testWord> GetWordsOrderedForRepetition(List<testWord> data, int needQty, int currLessonNum)
        {
            foreach (var item in data)
            {
                item.CurrWeight = (5 - item.Grade) * (currLessonNum - item.LastRepeat);
            }

            var result = data
                .OrderByDescending(x => x.CurrWeight)
                .ThenBy(x=>x.Rate)
                .ToList();

            return result;
        }

        private List<testWord> InitTestGetRepetitionData(int qty)
        {
            int step = qty <= 4 ? 1 : qty / 4;
            
            var result = new List<testWord>();
            var rate = 0;
            for (int i = 1; i <= qty; i++)
            {
                rate++;
                result.Add(new testWord
                {
                    Rate = rate,
                    Text = $"word_{rate}",
                    Grade = ((i - 1) / step) + 1
                });

                //rate++;
                //result.Add(new testWord
                //{
                //    Rate = rate,
                //    Text = $"word_{rate}",
                //    Grade = ((i - 1) / step) + 1
                //});
            }

            foreach (var word in result)
            {
                word.Phrases = new List<testPhrase>();
                for (int i = 0; i < 3; i++)
                {
                    word.Phrases.Add(new testPhrase
                    {
                        Order = i,
                        Text = $"phrase {i + 1} ({word.Text})",
                        Id = i + 1
                    });
                }
            }

            return result;
        }

        public class testWord
        {
            public string Text { get; set; }
            public int Rate { get; set; }
            public int Grade { get; set; }
            public int LastRepeat { get; set; } // last lesson number (order, not id)
            public int QtyRepeat { get; set; } // tmp
            public int CurrWeight { get; set; } // tmp
            public List<testPhrase> Phrases { get; set; } // tmp
            public int LastPhraseRepeatId { get; set; }
        }

        public class testPhrase
        {
            public int Id { get; set; }
            public string Text { get; set; }
            public int Order { get; set; }
        }

        public class testLesson
        {
            public int Num { get; set; }
            public List<testWord> WordsInLesson { get; set; }
            public List<testWord> WordsForRepetition { get; set; }
            public List<testPhrase> PhrasesForRepetition { get; set; }
            public string CurrState { get; set; }
        }

        #endregion test calc repetition list
    }
}

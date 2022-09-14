using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using NAudio.Wave;
using Microsoft.Extensions.Configuration;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authorization;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Extentions;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WordsForPhraseStateController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private TranslationManager _translationManager;
        private WordManager _wordManager;

        public WordsForPhraseStateController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            _wordManager = new WordManager(_context, _configuration, _hostingEnvironment);
            this.UserManager = userManager;
        }

        private readonly List<int> SchemaOfWordRepInWordPhrases = new List<int>() { 8, 16, 32, 64, 128, 256, 512, 1024 };

        [HttpGet]
        [ActionName("GetNext")]
        public async Task<IActionResult> GetNext()
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var wordsForPhraseState = _context.WordsForPhraseStates.AsNoTracking()
                .FirstOrDefault(x=>x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID);

            if (wordsForPhraseState == null)
            {
                var data = new WordsForPhraseStateData()
                {
                    ExcludeFromWaiting = new List<int>(),
                    CurrentWord = new Word(),
                    Waiting = new List<Word>()
                };

                wordsForPhraseState = new WordsForPhraseState()
                {
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID,
                    WordsForPhraseStateData = JsonConvert.SerializeObject(data),
                };

                _context.Update(wordsForPhraseState);
                _context.SaveChanges();
            }

            var wordsForPhraseStateData = JsonConvert.DeserializeObject<WordsForPhraseStateData>(wordsForPhraseState.WordsForPhraseStateData);

            // get word to process
            var words = _context.Words.AsNoTracking()
                    .Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                    .OrderBy(x => x.Rate)
                    .ToList();

            var currWordIndex = words.FindIndex(x => x.WordID == wordsForPhraseStateData.CurrentWord.WordID);
            if (currWordIndex >= 0 && (currWordIndex + 1) < words.Count)
            {
                currWordIndex++;
            }
            else
            {
                currWordIndex = 0;
            }

            var currWord = words[currWordIndex];

            // get list of already waiting words
            //var waitIds = wordsForPhraseStateData.Waiting.Select(x => x.Id).ToList();
            //var candidates = words
            //    .Where(x => 
            //        waitIds.Contains(x.WordID) 
            //        && !wordsForPhraseStateData.ExcludeFromWaiting.Contains(x.WordID)
            //        ).ToList();

            // get words from schema
            foreach (var val in this.SchemaOfWordRepInWordPhrases)
            {
                var wordIndex = val * 5;
                var index = currWordIndex - wordIndex;
                if (index >= 0)
                {
                    if (!wordsForPhraseStateData.Waiting.Any(x => x.WordID == words[index].WordID))
                    {
                        wordsForPhraseStateData.Waiting.Add(words[index]);
                    }
                }
            }

            wordsForPhraseStateData.CurrentWord = currWord;

            return Ok(wordsForPhraseStateData);
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] WordsForPhraseStateData wordsForPhraseStateData)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            if (!string.IsNullOrWhiteSpace(wordsForPhraseStateData.PhraseTexts))
            {
                var word = _context.Words.FirstOrDefault(x => x.WordID == wordsForPhraseStateData.CurrentWord.WordID);

                // save phrases to this word
                var arr = wordsForPhraseStateData.PhraseTexts.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                foreach (var phraseText in arr)
                {
                    if (string.IsNullOrEmpty(phraseText.Trim()))
                        continue;

                    WordPhrase wordPhrase = new WordPhrase()
                    {
                        LanguageID = (int)spaAppSetting.LearnLanguage.LanguageID,
                        Text = phraseText.FirstCharToUpper().LastCharToDot()
                    };

                    wordPhrase.HashCode = wordPhrase.GetHashCode();
                    WordPhraseWord wordPhraseWord = new WordPhraseWord()
                    {
                        WordID = word.WordID,
                        WordPhrase = wordPhrase,
                        PhraseOrder = 2000
                    };

                    wordPhrase.WordPhraseWords.Add(wordPhraseWord);
                    word.WordPhraseWords.Add(wordPhraseWord);
                    _context.WordPhrases.Add(wordPhrase);
                    _context.SaveChanges();
                }

                wordsForPhraseStateData.PhraseTexts = "";
            }

            var wordsForPhraseState = _context.WordsForPhraseStates
                .FirstOrDefault(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID);

            wordsForPhraseStateData.GoToRate = null;
            wordsForPhraseStateData.PhraseTexts = "";
            wordsForPhraseState.WordsForPhraseStateData = JsonConvert.SerializeObject(wordsForPhraseStateData);

            _context.Update(wordsForPhraseState);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [ActionName("GoToRate")]
        public async Task<IActionResult> GoToRate([FromBody] WordsForPhraseStateData wordsForPhraseStateData)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var wordsForPhraseState = _context.WordsForPhraseStates
                .FirstOrDefault(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID);

            // get word to process
            var words = _context.Words.AsNoTracking()
                    .Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                    .OrderBy(x => x.Rate)
                    .ToList();

            var currWordIndex = words.FindIndex(x => x.Rate == wordsForPhraseStateData.GoToRate);
            var currWord = words[currWordIndex];

            // get words from schema
            foreach (var val in this.SchemaOfWordRepInWordPhrases)
            {
                var wordIndex = val * 5;
                var index = currWordIndex - wordIndex;
                if (index >= 0)
                {
                    if (!wordsForPhraseStateData.Waiting.Any(x => x.WordID == words[index].WordID))
                    {
                        wordsForPhraseStateData.Waiting.Add(words[index]);
                    }
                }
            }

            wordsForPhraseStateData.CurrentWord = currWord;
            wordsForPhraseStateData.GoToRate = null;

            return Ok(wordsForPhraseStateData);
        }
    }
}


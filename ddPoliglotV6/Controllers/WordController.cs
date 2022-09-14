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
using ddPoliglotV6.BL.Helpers;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class WordController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private TranslationManager _translationManager;
        private WordManager _wordManager;

        public WordController(ddPoliglotDbContext context,
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

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] ListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "Rate";
            }
            else
            {
                // first letter to upper case
                args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            }

            IQueryable<Word> query = _context.Words.AsNoTracking().Where(x =>x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted);
            if (args.Order == "desc")
            {
                query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
            }
            else 
            {
                query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
            }

            // make query
            var result = new ListResult<Word>()
            {
                Count = await query.CountAsync(),
                Data = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync()
            };

            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var result = await _context.Words.FirstOrDefaultAsync(x=>x.WordID == id);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] Word word)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            Word oldItem;
            if ( word.WordID == 0)
            {
                oldItem = new Word
                {
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    Pronunciation = word.Pronunciation,
                    Rate = word.Rate,
                    Text = word.Text,
                };

                using (var ent = new ddPoliglotDbContext(_configuration)) {
                    ent.Add(oldItem);
                    ent.SaveChanges();
                }
            }
            else
            {
                // get old article version
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    oldItem = ent.Words.Where(x => x.WordID == word.WordID).FirstOrDefault();
                }
            }

            if (
                oldItem.Text != word.Text
                || oldItem.Rate != word.Rate
                || oldItem.Pronunciation != word.Pronunciation
                )
            {
                oldItem.Rate = word.Rate;
                oldItem.Pronunciation = word.Pronunciation;
                oldItem.Text = word.Text;
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    ent.Update(oldItem);
                    ent.SaveChanges();
                }
            }

            _wordManager.SpeechForWordUpdate(oldItem);

            return Ok(oldItem);
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] Word word)
        {
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                var oldItem = ent.Words.Where(x => x.WordID == word.WordID).FirstOrDefault();
                ent.Remove(oldItem);
                ent.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        [ActionName("MakeSpeeches")]
        public async Task<IActionResult> MakeSpeeches([FromBody] Word word)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var oldItem = _context.Words.Where(x => x.WordID == word.WordID).FirstOrDefault();
            _wordManager.SpeechForWordUpdate(oldItem);

            var wordTranslation = _context.WordTranslations.Where(x => x.WordID == word.WordID 
                && x.LanguageID == spaAppSetting.NativeLanguage.LanguageID).FirstOrDefault();

            if (wordTranslation != null)
            {
                _wordManager.SpeechForWordTranslationUpdate(wordTranslation);
            }

            var wordPhrasies = (from wpw in _context.WordPhraseWords.AsNoTracking()
                        join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                        join wpt in _context.WordPhraseTranslations on new { wp.WordPhraseID, LanguageID = spaAppSetting.NativeLanguage.LanguageID } equals new { wpt.WordPhraseID, wpt.LanguageID } into grouping1
                        from wptj in grouping1.DefaultIfEmpty()
                        where wpw.WordID == word.WordID && wpw.PhraseOrder > 0
                        select new WordPhrase()
                        {
                            LanguageID = wp.LanguageID,
                            Text = wp.Text,
                            HashCode = wp.HashCode,
                            HashCodeSpeed1 = wp.HashCodeSpeed1,
                            HashCodeSpeed2 = wp.HashCodeSpeed2,
                            SpeachDuration = wp.SpeachDuration,
                            SpeachDurationSpeed1 = wp.SpeachDurationSpeed1,
                            SpeachDurationSpeed2 = wp.SpeachDurationSpeed2,
                            TextSpeechFileName = wp.TextSpeechFileName,
                            TextSpeechFileNameSpeed1 = wp.TextSpeechFileNameSpeed1,
                            TextSpeechFileNameSpeed2 = wp.TextSpeechFileNameSpeed2,
                            WordsUsed = wp.WordsUsed,
                            WordPhraseID = wp.WordPhraseID,
                            WordPhraseTranslation = wptj,
                            PhraseOrderInCurrentWord = wpw.PhraseOrder,
                            CurrentWordID = wpw.WordID
                        }).ToList();

            foreach (var wordPhrase in wordPhrasies)
            {
                _wordManager.SpeechForWordPhraseUpdate(wordPhrase);
                if (wordPhrase.WordPhraseTranslation != null)
                {
                    _wordManager.SpeechForWordPhraseTranslationUpdate(wordPhrase.WordPhraseTranslation);
                }
            }

            _wordManager.UpdateWordPhrasiesStateForLessons(word.WordID, spaAppSetting);

            return Ok();
        }

        [HttpGet]
        [ActionName("GetRecomended")]
        public async Task<IActionResult> GetRecomended([FromQuery] SearchListArg args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            // get next 5 words which are ready to learn (translated, have at least 3 phrase with order, all are speeched)
            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID }
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    WordID = w.WordID,
                    WordTranslation = wt,
                    WordUser = wuj,
                };

            query = query.Where(x => (x.TextSpeechFileName ?? "") != ""); // word speeched
            query = query.Where(x => (x.WordTranslation.TextSpeechFileName ?? "") != ""); // word translation speeched
            query = query.Where(x => x.WordTranslation.ReadyForLessonPhrasiesCnt >= 3); // has at least 3 ready phrases
            query = query.Where(x => x.WordUser == null || (x.WordUser != null && x.WordUser.Grade <= 3)); // has grade up to 3

            var resData = await query.Take(5).ToListAsync();

            _wordManager.FillWordListWithPhrases(resData, spaAppSetting, true);

            var result = new ListResult<Word>()
            {
                Count = -1,
                Data = resData
            };

            return Ok(result);
        }


        [HttpPost]
        [ActionName("GetFillRecomended1")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetFillRecomended1([FromBody] UserLessonArgs args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var wordsAndWordsRepIds = args.wordIDs;

            // words and repetitionin one list
            wordsAndWordsRepIds.AddRange(args.repeatWordIds);

            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID }
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    WordID = w.WordID,
                    OxfordLevel = w.OxfordLevel,
                    WordTranslation = wt,
                    WordUser = wuj,
                };

            query = query.Where(x => wordsAndWordsRepIds.Contains(x.WordID));
            query = query.OrderBy(x => x.Rate);

            var resData = await query.ToListAsync();

            _wordManager.FillWordListWithPhrases(resData, spaAppSetting, true);

            var words = resData.Where(x => args.wordIDs.Contains(x.WordID));
            var wordsForRepetition = resData.Where(x => args.repeatWordIds.Contains(x.WordID));

            // in rep words set only one phrase which is need
            var lst = new List<(int wordId, int wordPhraseId)>();
            foreach (var sdata in args.repeatWordPhrasesData)
            {
                var ar = sdata.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                var wordId = Convert.ToInt32(ar[0]);
                var wordPhraseId = Convert.ToInt32(ar[1]);
                lst.Add((wordId: wordId, wordPhraseId: wordPhraseId));
            }

            foreach (var word in wordsForRepetition)
            {
                var wordPhraseId = lst.FirstOrDefault(x => x.wordId == word.WordID).wordPhraseId;
                word.WordPhraseSelected = word.WordPhraseSelected.Where(x => x.WordPhraseID == wordPhraseId).ToList();
            }

            var result = new
            {
                words = words,
                wordsForRepetition = wordsForRepetition,
            };

            return Ok(result);
        }


        [HttpGet]
        [ActionName("GetRecomendedCandidates1")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetRecomendedCandidates1([FromQuery] SearchListArg args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID }
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    WordID = w.WordID,
                    OxfordLevel = w.OxfordLevel,
                    WordTranslation = wt,
                    WordUser = wuj,
                };

            query = query.Where(x => (x.TextSpeechFileName ?? "") != ""); // word speeched
            query = query.Where(x => (x.WordTranslation.TextSpeechFileName ?? "") != ""); // word translation speeched
            query = query.Where(x => x.WordTranslation.ReadyForLessonPhrasiesCnt >= 3); // has at least 3 ready phrases
            query = query.Where(x => x.WordUser == null || (x.WordUser != null && x.WordUser.Grade == 0)); // has not grade or grade is 0
            query = query.OrderBy(x => x.Rate);

            var resData = await query.Take(150).ToListAsync();

            foreach (var item in resData)
            {
                item.WordPhraseSelected = new List<WordPhrase>();
            }

            var lastUserLesson = await _context.UserLessons.AsNoTracking()
                .Where(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .OrderByDescending(x => x.Num)
                .Take(1)
                .ToListAsync();

            var lastUserLessonNum = lastUserLesson.Count == 0 ? 0 : lastUserLesson[0].Num;


            var result = new
            {
                lastUserLessonNum = lastUserLessonNum,
                words = resData,
                repetition = await _wordManager.GetListForRepetition1(spaAppSetting, userId, lastUserLessonNum, 10)
            };

            return Ok(result);
        }


        [HttpGet]
        [ActionName("GetFiltered")]
        public async Task<IActionResult> GetFiltered([FromQuery] SearchListArg args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID } into grouping1
                from wtj in grouping1.DefaultIfEmpty()
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                select new Word()
                {
                    LanguageID = w.LanguageID,
                    Pronunciation = w.Pronunciation,
                    Rate = w.Rate,
                    Text = w.Text,
                    HashCode = w.HashCode,
                    HashCodeSpeed1 = w.HashCodeSpeed1,
                    HashCodeSpeed2 = w.HashCodeSpeed2,
                    SpeachDuration = w.SpeachDuration,
                    SpeachDurationSpeed1 = w.SpeachDurationSpeed1,
                    SpeachDurationSpeed2 = w.SpeachDurationSpeed2,
                    TextSpeechFileName = w.TextSpeechFileName,
                    TextSpeechFileNameSpeed1 = w.TextSpeechFileNameSpeed1,
                    TextSpeechFileNameSpeed2 = w.TextSpeechFileNameSpeed2,
                    TextClear = w.TextClear,
                    WordID = w.WordID,
                    WordTranslation = wtj,
                    WordUser = wuj,
                };

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "Rate";
            }
            else
            {
                // first letter to upper case
                args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            }

            if (!string.IsNullOrEmpty(args.searchText))
            {
                query = query.Where(x => x.Text.Contains(args.searchText));
            }

            if (args.RateFrom > 0)
            {
                query = query.Where(x => x.Rate >= args.RateFrom);
            }

            if (args.RateTo > 0)
            {
                query = query.Where(x => x.Rate <= args.RateTo);
            }

            if (args.GradeFrom > 0)
            {
                query = query.Where(x => x.WordUser != null && x.WordUser.Grade >= args.GradeFrom);
            }

            if (args.GradeTo > 0)
            {
                query = query.Where(x => x.WordUser == null || x.WordUser.Grade <= args.GradeTo);
            }

            if (args.Order == "desc")
            {
                if (args.Sort.ToLower() == "grade")
                {
                    query = query.OrderByDescending(p => p.WordUser.Grade);
                }
                else
                {
                    query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
                }
            }
            else
            {
                if (args.Sort.ToLower() == "grade")
                {
                    query = query.OrderBy(p => p.WordUser.Grade);
                }
                else
                {
                    query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
                }
            }

            var resData = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync();

            _wordManager.FillWordListWithPhrases(resData, spaAppSetting, false);

            // make query
            var result = new ListResult<Word>()
            {
                Count = await query.CountAsync(),
                Data = resData
            };

            return Ok(result);
        }

        [HttpPost]
        [ActionName("getWordPhrases")]
        public async Task<IActionResult> GetWordPhrases([FromBody] SearchListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            var query = from wpw in _context.WordPhraseWords.AsNoTracking()
                            join wp in _context.WordPhrases.AsNoTracking() on wpw.WordPhraseID equals wp.WordPhraseID
                            join wpt in _context.WordPhraseTranslations on new { wp.WordPhraseID, LanguageID = spaAppSetting.NativeLanguage.LanguageID } equals new { wpt.WordPhraseID, wpt.LanguageID } into grouping1
                                from wptj in grouping1.DefaultIfEmpty()
                        where wpw.WordID == args.parentID
                        select new WordPhrase() 
                        { 
                            LanguageID = wp.LanguageID,
                            Text = wp.Text,
                            HashCode = wp.HashCode,
                            HashCodeSpeed1 = wp.HashCodeSpeed1,
                            HashCodeSpeed2 = wp.HashCodeSpeed2,
                            SpeachDuration = wp.SpeachDuration,
                            SpeachDurationSpeed1 = wp.SpeachDurationSpeed1,
                            SpeachDurationSpeed2 = wp.SpeachDurationSpeed2,
                            TextSpeechFileName = wp.TextSpeechFileName,
                            TextSpeechFileNameSpeed1 = wp.TextSpeechFileNameSpeed1,
                            TextSpeechFileNameSpeed2 = wp.TextSpeechFileNameSpeed2,
                            WordsUsed = wp.WordsUsed,
                            WordPhraseID = wp.WordPhraseID,
                            WordPhraseTranslation = wptj,
                            PhraseOrderInCurrentWord = wpw.PhraseOrder,
                            CurrentWordID = wpw.WordID
                        };

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "PhraseOrderInCurrentWord";
                args.Order = "desc";
            }
            else
            {
                // first letter to upper case
                args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            }

            if (args.Order == "desc")
            {
                query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
            }
            else
            {
                query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
            }

            var result = new ListResult<WordPhrase>()
            {
                Count = await query.CountAsync(),
                Data = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync()
            };

            return Ok(result);
        }

        [HttpPost]
        [ActionName("updateWordTranslation")]
        public async Task<IActionResult> UpdateWordTranslation([FromBody] WordTranslation wordTranslation)
        {
            if (wordTranslation.WordTranslationID > 0)
            {
                _context.Update(wordTranslation);
            }
            else
            {
                _context.Add(wordTranslation);
            }

            await _context.SaveChangesAsync();

            _wordManager.SpeechForWordTranslationUpdate(wordTranslation);

            return Ok(wordTranslation);
        }

        [HttpPost]
        [ActionName("updateWordPhraseTranslation")]
        public async Task<IActionResult> UpdateWordPhraseTranslation([FromBody] WordPhraseTranslation wordPhraseTranslation)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            if (string.IsNullOrEmpty(wordPhraseTranslation.Text) && wordPhraseTranslation.WordPhrase != null)
            {
                // translate if translation is empty
                wordPhraseTranslation.Text = _translationManager.Translate(new TranslateArg() 
                { 
                    SourceLanguage = spaAppSetting.LearnLanguage.Code,
                    TargetLanguage = spaAppSetting.NativeLanguage.Code,
                    SourceText = wordPhraseTranslation.WordPhrase.Text
                });
            }

            wordPhraseTranslation.WordPhrase = null;

            if (wordPhraseTranslation.WordPhraseTranslationID > 0)
            {
                _context.Update(wordPhraseTranslation);
            }
            else
            {
                _context.Add(wordPhraseTranslation);
            }

            await _context.SaveChangesAsync();

            return Ok(wordPhraseTranslation);
        }

        [HttpPost]
        [ActionName("SaveWordPhrase")]
        public async Task<IActionResult> SaveWordPhrase([FromBody] WordPhrase wordPhrase)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var voice = AudioHelper.GetDefaultVoiceByLanguage(wordPhrase.LanguageID);

            try
            {
                WordPhrase oldItem;
                if (wordPhrase.WordPhraseID == 0)
                {
                    oldItem = new WordPhrase
                    {
                        SourceType = 2,
                        WordsUsed = wordPhrase.WordsUsed,

                        LanguageID = wordPhrase.LanguageID,
                        Text = wordPhrase.Text,
                    };

                    oldItem.HashCode = HashManager.GetHashFromTextAndVoice(oldItem.Text
                        , voice.ln
                        , voice.voice
                        , (decimal)voice.pitch
                        , (decimal)voice.speed);

                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        ent.Add(oldItem);
                        ent.SaveChanges();

                        // connect to words
                        var arr = oldItem.WordsUsed.Split(',');
                        foreach (var wordStr in arr)
                        {
                            var arr2 = wordStr.Split(';');
                            var wordId = Convert.ToInt32(arr2[1]);
                            ent.Add(new WordPhraseWord()
                            {
                                WordID = wordId,
                                WordPhraseID = oldItem.WordPhraseID,
                            });
                        }

                        ent.SaveChanges();
                    }
                }
                else
                {
                    // get old version
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        oldItem = ent.WordPhrases.Where(x => x.WordPhraseID == wordPhrase.WordPhraseID).FirstOrDefault();
                    }
                }

                if (
                    oldItem.Text != wordPhrase.Text
                    )
                {
                    oldItem.Text = wordPhrase.Text;
                    oldItem.HashCode = HashManager.GetHashFromTextAndVoice(oldItem.Text
                        , voice.ln
                        , voice.voice
                        , (decimal)voice.pitch
                        , (decimal)voice.speed);

                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        ent.Update(oldItem);
                        ent.SaveChanges();
                    }
                }

                // save phraseOrder if need
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    var wordPhraseWord = ent.WordPhraseWords.FirstOrDefault(x => x.WordID == wordPhrase.CurrentWordID && x.WordPhraseID == oldItem.WordPhraseID);
                    wordPhraseWord.PhraseOrder = wordPhrase.PhraseOrderInCurrentWord;
                    ent.SaveChanges();
                }

                _wordManager.SpeechForWordPhraseUpdate(oldItem);

                // connected word translation
                WordPhraseTranslation oldWordPhraseTranslation;
                if (wordPhrase.WordPhraseTranslation != null
                    && wordPhrase.WordPhraseTranslation.WordPhraseTranslationID > 0)
                {
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        oldWordPhraseTranslation = ent.WordPhraseTranslations.Where(x => x.WordPhraseTranslationID == wordPhrase.WordPhraseTranslation.WordPhraseTranslationID).FirstOrDefault();
                    }

                    oldWordPhraseTranslation.Text = wordPhrase.WordPhraseTranslation.Text;
                    oldWordPhraseTranslation.WordPhrase = null;
                }
                else 
                {
                    oldWordPhraseTranslation = new WordPhraseTranslation()
                    {
                        LanguageID = spaAppSetting.NativeLanguage.LanguageID,
                        Text = !string.IsNullOrEmpty(wordPhrase.WordPhraseTranslation?.Text)
                          ? wordPhrase.WordPhraseTranslation?.Text
                          : "",
                        WordPhraseID = oldItem.WordPhraseID
                    };
                }

                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    if (oldWordPhraseTranslation.WordPhraseTranslationID > 0)
                    {
                        ent.Update(oldWordPhraseTranslation);
                        ent.SaveChanges();
                    }
                    else
                    {
                        if (!string.IsNullOrWhiteSpace(oldWordPhraseTranslation.Text))
                        {
                            ent.Add(oldWordPhraseTranslation);
                            ent.SaveChanges();
                        }
                    }
                }

                _wordManager.SpeechForWordPhraseTranslationUpdate(oldWordPhraseTranslation);

                oldItem.CurrentWordID = wordPhrase.CurrentWordID;
                oldItem.PhraseOrderInCurrentWord = wordPhrase.PhraseOrderInCurrentWord;

                oldItem.WordPhraseTranslation = oldWordPhraseTranslation;

                _wordManager.UpdateWordPhrasiesStateForLessons(wordPhrase.CurrentWordID, spaAppSetting);

                return Ok(oldItem);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [ActionName("AddListFromTextWithPhrases")]
        public async Task<IActionResult> AddListFromTextWithPhrases([FromBody] SearchListArg args)
        {
            if (args.int1 == 0)
            {
                await _wordManager.AddListFromTextWithPhrases(args.str1, new SpaAppSetting(HttpContext.Request.Headers, _context), args.Recomended);
            }
            else if(args.int1 == 1)
            {
                await _wordManager.AddListFromTextWithPhrasesV3(args.str1, new SpaAppSetting(HttpContext.Request.Headers, _context), args.Recomended);
            }
            else if (args.int1 == 3)
            {
                await _wordManager.AddListFromTextWithPhrasesV4(args.str1, new SpaAppSetting(HttpContext.Request.Headers, _context), args.Recomended);
            }

            return Ok();
        }

        [HttpPost]
        [ActionName("getListForRepetition")]
        public async Task<IActionResult> GetListForRepetition([FromBody] SearchListArg args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var baseName = "";
            if (!string.IsNullOrEmpty(args.str1) && args.str1.IndexOf('_') > 0)
            {
                var arr = args.str1.Split("_");
                if (arr.Length > 1)
                {
                    baseName = arr[0] + "_";
                }
            }

            var result = await _wordManager.GetListForRepetition(spaAppSetting, userId, args.parentID, baseName, args.int1);
            return Ok(result);
        }

        [HttpGet]
        [ActionName("GetDictionary")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetDictionary([FromQuery] int version)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var dictionaryVersions = _context.DictionaryVersions.Where(x=> 
                x.LanguageID == spaAppSetting.LearnLanguage.LanguageID 
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                ).ToList();

            if (dictionaryVersions.Count == 0)
            {
                var dictionaryVersion = new DictionaryVersion()
                {
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    NativeLanguageID = spaAppSetting.NativeLanguage.LanguageID,
                    Value = 7,
                    Description = "first request"
                };

                _context.DictionaryVersions.Add(dictionaryVersion);
                _context.SaveChanges();
                dictionaryVersions.Add(dictionaryVersion);
            }
            var ww = _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID).ToList();

            var lastVersion = dictionaryVersions.Max(x=> x.Value);
            if (lastVersion == version)
                return Ok(new { version = lastVersion, dictWords = new List<object>(), userLessons = new List<object>() });

            // var ww = _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID).ToList();
            var query =
                //from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                //join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID }
                //join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                //from wuj in grouping2.DefaultIfEmpty()
                //where (w.TextSpeechFileName ?? "") != ""
                //&& (wt.TextSpeechFileName ?? "") != ""
                //&& wt.ReadyForLessonPhrasiesCnt >= 3
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID && !x.Deleted)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID } into grouping1
                from wtj in grouping1.DefaultIfEmpty()
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()

                orderby w.Rate
                select new
                {
                    wordID = w.WordID,
                    text = w.Text,
                    rate = w.Rate,
                    pronunciation = w.Pronunciation,
                    translation = wtj.Text,
                    oxfordLevel = w.OxfordLevel,
                    grade = wuj == null ? 0 : wuj.Grade,
                    wordUserID = wuj == null ? 0 : wuj.WordUserID,
                    lastRepeatInArticleByParamID = wuj == null ? 0 : wuj.LastRepeatInArticleByParamID,
                    lastRepeatInLessonNum = wuj == null ? 0 : wuj.LastRepeatInLessonNum,
                    lastRepeatWordPhraseId = wuj == null ? 0 : wuj.LastRepeatWordPhraseId,
                    sourceType = wuj == null ? 0 : wuj.SourceType,
                    textClear = w.TextClear,
                    readyForLesson = (
                        (w.TextSpeechFileName ?? "") != ""
                        && (wtj.TextSpeechFileName ?? "") != ""
                        && wtj.ReadyForLessonPhrasiesCnt >= 3
                        ) ? 1 : 0
            };

            var dictWords = await query.ToListAsync();

            var userLessons = await _context.UserLessons.AsNoTracking()
                .Where(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .Include(x=>x.UserLessonWords)
                .OrderByDescending(x => x.Num)
                .ToListAsync();


            return Ok(new { version = lastVersion, dictWords = dictWords, userLessons = userLessons });
        }
    }
}


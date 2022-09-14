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
using Microsoft.AspNetCore.Identity;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserLessonController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private TranslationManager _translationManager;
        private WordManager _wordManager;

        public UserLessonController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
        )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            _wordManager = new WordManager(_context, _configuration, _hostingEnvironment);
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userLessons = await _context.UserLessons.AsNoTracking().Include(x=>x.UserLessonWords).ThenInclude(x=>x.Word)
                .Where(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .OrderByDescending(x => x.Num)
                .ToListAsync();

            var words = new List<Word>();
            foreach (var userLesson in userLessons)
            {
                foreach (var userLessonWord in userLesson.UserLessonWords)
                {
                    words.Add(userLessonWord.Word);
                }
            }

            var wordIds = words.Select(x => x.WordID).ToList();

            // apply wordUsers
            var wordUsers = _context.WordUsers
                .Where(x =>
                    wordIds.Contains(x.WordID)
                    && x.UserID == userId
                    && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .ToList();

            foreach (var word in words.OrderBy(x => x.WordID))
            {
                word.WordUser = wordUsers.FirstOrDefault(x=>x.WordID == word.WordID);
            }

            return Ok(new { userLessons = userLessons });
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("GetLast")]
        public async Task<IActionResult> GetLast()
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var result = await _context.UserLessons.AsNoTracking()
                .Where(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                .OrderByDescending(x => x.Num)
                .Take(1)
                .ToListAsync();
            return Ok(result);
        }

        

        [HttpPost]
        [ActionName("CreateNext")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> CreateNext([FromBody] UserLessonArgs args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var isNew = false;

            UserLesson userLesson = await _context.UserLessons.FirstOrDefaultAsync(x => x.Num == args.lessonNum && x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID);
            if (userLesson == null)
            {
                isNew = true;
                userLesson = new UserLesson()
                {
                    Created = DateTime.Now,
                    Updated = DateTime.Now,
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    Num = args.lessonNum,
                    UserID = userId,
                    UserLessonWords = new List<UserLessonWord>(),
                    LessonType = args.lessonType,
                };

                _context.UserLessons.Add(userLesson);
                await _context.SaveChangesAsync();
            }

            var wordsAndWordsRepIds = new List<int>();

            if (isNew)
            {
                // exclude words in repetition which are in main list
                args.repeatWordIds = args.repeatWordIds.Where(x => !args.wordIDs.Contains(x)).Distinct().ToList();

                // add words and words repetition to this lesson
                foreach (var wordId in args.wordIDs)
                {
                    userLesson.UserLessonWords.Add(new UserLessonWord()
                    {
                        UserLessonID = userLesson.UserLessonID,
                        WordID = wordId,
                        wordType = 0,
                    });
                }

                foreach (var wordId in args.repeatWordIds)
                {
                    userLesson.UserLessonWords.Add(new UserLessonWord()
                    {
                        UserLessonID = userLesson.UserLessonID,
                        WordID = wordId,
                        wordType = 1, // mark as repetition
                    });
                }

                await _context.SaveChangesAsync();
            }

            wordsAndWordsRepIds.AddRange(args.wordIDs);

            // words and repetitionin ids one list
            wordsAndWordsRepIds.AddRange(args.repeatWordIds);
            

            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
                join wt in _context.WordTranslations.AsNoTracking() on new { w.WordID, spaAppSetting.NativeLanguage.LanguageID } equals new { wt.WordID, wt.LanguageID }
                join wu in _context.WordUsers.AsNoTracking() on new { w.WordID, UserID = userId } equals new { wu.WordID, wu.UserID } into grouping2
                from wuj in grouping2.DefaultIfEmpty()
                where wordsAndWordsRepIds.Contains(w.WordID)
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

            var resData = await query.ToListAsync();

            _wordManager.FillWordListWithPhrases(resData, spaAppSetting, true);

            if (resData.Count == 0)
            {
                throw new Exception($"No data for lesson {args.lessonNum} {userId}");
            }

            foreach (var item in resData)
            {
                if (item.WordPhraseSelected.Count < 3)
                {
                    throw new Exception($"not enouph phrases wor word <{item.Text} / {item.WordID}>");
                }
            }

            var words = resData.Where(x => args.wordIDs.Contains(x.WordID));
            var wordsForRepetition = resData.Where(x => args.repeatWordIds.Contains(x.WordID));

            // calculate repetition phrases and mark phrase is used in repetition
            foreach (var word in wordsForRepetition)
            {
                var phrase = _wordManager.GetPhraseForRepetition(word);
                word.WordPhraseSelected = new List<WordPhrase>();
                if (phrase != null)
                {
                    word.WordPhraseSelected.Add(phrase);

                    if (isNew) {
                        word.WordUser.LastRepeatInArticleByParamID = userLesson.UserLessonID;
                        word.WordUser.LastRepeatInLessonNum = userLesson.Num;
                        word.WordUser.LastRepeatWordPhraseId = phrase.WordPhraseID;
                        _context.Update(word.WordUser);
                    }
                }
            }

            if (isNew)
            {
                // set grade 1 to all words in this lesson
                foreach (var word in words)
                {
                    var wordUser = word.WordUser;
                    if (wordUser == null)
                    {
                        wordUser = new WordUser()
                        {
                            WordID = word.WordID,
                            Grade = 1,
                            LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                            UserID = userId,
                        };
                    }
                    else
                    {
                        wordUser.Grade = 1;
                    }

                    if (wordUser.WordUserID == 0)
                    {
                        _context.Add(wordUser);
                    }
                    else
                    {
                        _context.Update(wordUser);
                    }

                    word.WordUser = wordUser;
                }

                _context.SaveChanges();
            }
            else {
                userLesson.UserLessonWords = await _context.UserLessonWords
                    .AsNoTracking()
                    .Where(x => x.UserLessonID == userLesson.UserLessonID)
                    .ToListAsync();
            }

            
            var result = new
            {
                userLesson = userLesson,
                words = words,
                wordsForRepetition = wordsForRepetition,
            };

            return Ok(result);
        }

        [HttpPost]
        [ActionName("Update")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Update([FromBody] UserLesson args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userLesson = await _context.UserLessons
                .FirstOrDefaultAsync(x => x.UserLessonID == args.UserLessonID);
            userLesson.TotalSeconds = args.TotalSeconds;
            userLesson.LearnSeconds = args.LearnSeconds;
            userLesson.Updated = DateTime.Now;
            userLesson.Finished = DateTime.Now;
            _context.Update(userLesson);

            await _context.SaveChangesAsync();
            userLesson.UserLessonWords = new List<UserLessonWord>();
            return Ok(userLesson);
        }
    }
}

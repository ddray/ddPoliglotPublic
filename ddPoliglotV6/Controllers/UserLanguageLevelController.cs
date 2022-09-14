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
    public class UserLanguageLevelController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly RoleManager<IdentityRole> RoleManager;
        private TranslationManager _translationManager;

        public UserLanguageLevelController(ddPoliglotDbContext context,
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
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpGet]
        [ActionName("Get1")]
        public async Task<IActionResult> Get1()
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var result = _context.UserLanguageLevels
                .FirstOrDefault(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID);
            return Ok(result);
        }

        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [HttpPost]
        [ActionName("setByLevel1")]
        public async Task<IActionResult> SetByLevel1([FromBody] int level )
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var oldLevel = 0;

            // 1 - update level for tis user
            var oldItem = _context.UserLanguageLevels
                .FirstOrDefault(x => x.UserID == userId && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID);

            if (oldItem == null)
            {
                oldItem = new UserLanguageLevel()
                {
                    LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                    UserID = userId,
                    Level = level
                };

                _context.Add(oldItem);
            }
            else
            {
                oldLevel = oldItem.Level;
                oldItem.Level = level;
                _context.Update(oldItem);
            }

            await _context.SaveChangesAsync();

            // update wordUsers by this level
            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
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
                    WordUser = wuj,
                };

            foreach (var word in query.ToList())
            {
                if (word.OxfordLevel == 0)
                {
                    continue;
                }
                else if (word.OxfordLevel > level)
                {
                    if (word.WordUser != null)
                    {
                        // greate then new level
                        if (word.WordUser.SourceType == 1)
                        {
                            // greade was set by automat
                            word.WordUser.Grade = 0;
                            word.WordUser.LastRepeatInArticleByParamID = 0;
                            word.WordUser.LastRepeatInLessonNum = 0;
                            word.WordUser.LastRepeatWordPhraseId = 0;
                            word.WordUser.RepeatHistory = "";
                            _context.WordUsers.Update(word.WordUser);
                        }
                    }
                }
                else
                {
                    // less or equal  new level
                    // set all as known if not marked before by hand
                    if (word.WordUser == null)
                    {
                        var wordUser = new WordUser()
                        {
                            Grade = GetGradeFromAuthomat(word.OxfordLevel, level),
                            LanguageID = word.LanguageID,
                            SourceType = 1,
                            UserID = userId,
                            WordID = word.WordID,
                        };

                        _context.WordUsers.Add(wordUser);
                    }
                }
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        private int GetGradeFromAuthomat(int wordLevel, int level)
        {
            var result = 0;
            if (level == 1)
            {
                result = 1;
            }
            else if (level == 2)
            {
                if (wordLevel == 1)
                {
                    result = 2;
                }
                else
                {
                    result = 1;
                }
            }
            else if (level == 3)
            {
                if (wordLevel == 1)
                {
                    result = 3;
                }
                else if (wordLevel == 2)
                {
                    result = 2;
                }
                else
                {
                    result = 1;
                }
            }
            else if (level == 4)
            {
                if (wordLevel == 1)
                {
                    result = 4;
                }
                else if (wordLevel == 2)
                {
                    result = 3;
                }
                else if (wordLevel == 3)
                {
                    result = 2;
                }
                else
                {
                    result = 1;
                }
            }
            else if (level == 5)
            {
                if (wordLevel == 1)
                {
                    result = 5;
                }
                else if (wordLevel == 2)
                {
                    result = 4;
                }
                else if (wordLevel == 3)
                {
                    result = 3;
                }
                else if (wordLevel == 4)
                {
                    result = 2;
                }
                else
                {
                    result = 1;
                }
            }

            return result;
        }
    }
}

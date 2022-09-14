using System;
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
using System.Security.Claims;
using ddPoliglotV6.BL.Models;
using Microsoft.AspNetCore.Identity;
using ddPoliglotV6.BL.Constants;
using Newtonsoft.Json;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Helpers;
using Newtonsoft.Json.Serialization;
using System.Collections.Generic;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleByParamController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private TranslationManager _translationManager;
        private WordManager _wordManager;

        public ArticleByParamController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            this.UserManager = userManager;
            _translationManager = new TranslationManager(_context, _configuration, _hostingEnvironment);
            _wordManager = new WordManager(_context, _configuration, _hostingEnvironment);
        }

        [HttpGet]
        [ActionName("GetById")]
        public async Task<IActionResult> GetById([FromQuery] int id)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);

            var query = _context.ArticleByParams.Where(x => x.ArticleByParamID == id);

            return Ok(await query.FirstOrDefaultAsync());
        }

        [HttpGet]
        [ActionName("GetByIdWithReadyAudio")]
        public async Task<IActionResult> GetByIdWithReadyAudio([FromQuery] int id)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);



            if (spaAppSetting.LearnLanguage == null)
            {
                spaAppSetting.LearnLanguage = new Language()
                {
                    LanguageID = 1,
                    Code = "en",
                    CodeFull = "en-US",
                    Name = "English(US, GB)",
                    ShortName = "en"
                };

                spaAppSetting.NativeLanguage = new Language()
                {
                    LanguageID = 2,
                    Code = "ru",
                    CodeFull = "ru-RU",
                    Name = "Russian",
                    ShortName = "ru"
                };
            }

            return await GetByIdWithReadyAudioExec(id, spaAppSetting);
        }
        
        //[HttpGet]
        //[Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        //[ActionName("GetByLessonTypeWithReadyAudio")]
        //public async Task<IActionResult> GetByLessonTypeWithReadyAudio([FromQuery] int lessonType)
        //{
        //    var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
        //    var name = lessonType == 0 
        //        ? "en_mobile_normal_template" 
        //        : lessonType == 1 
        //            ? "en_mobile_short_template" 
        //            : "en_mobile_huge_template";

        //    var id = (await _context.ArticleByParams.AsNoTracking().FirstOrDefaultAsync(x => x.Name == name)).ArticleByParamID;
        //    return await GetByIdWithReadyAudioExec2(id, spaAppSetting);
        //}

        [HttpGet]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        [ActionName("GetByIdWithReadyAudio1")]
        public async Task<IActionResult> GetByIdWithReadyAudio1([FromQuery] int id)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);



            if (spaAppSetting.LearnLanguage == null)
            {
                spaAppSetting.LearnLanguage = new Language()
                {
                    LanguageID = 1,
                    Code = "en",
                    CodeFull = "en-US",
                    Name = "English(US, GB)",
                    ShortName = "en"
                };

                spaAppSetting.NativeLanguage = new Language()
                {
                    LanguageID = 2,
                    Code = "ru",
                    CodeFull = "ru-RU",
                    Name = "Russian",
                    ShortName = "ru"
                };
            }

            return await GetByIdWithReadyAudioExec(id, spaAppSetting);
        }


        private async Task<IActionResult> GetByIdWithReadyAudioExec(int id, SpaAppSetting spaAppSetting)
        {
            var query = _context.ArticleByParams.Where(x => x.ArticleByParamID == id);

            var data = await query.FirstOrDefaultAsync();

            var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(data.DataJson);
            if (articleByParamData != null)
            {
                var rootPath = _hostingEnvironment.WebRootPath;
                foreach (var mixParam in articleByParamData.MixParamsList)
                {
                    mixParam.firstDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.firstDictorPhrases);
                    mixParam.firstBeforeDialogDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.firstBeforeDialogDictorPhrases);
                    mixParam.beforeByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeByOrderMixDictorPhrases);
                    mixParam.insideByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideByOrderMixDictorPhrases);
                    mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeBaseWordsDirMixDictorPhrases);
                    mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideBaseWordsDirMixDictorPhrases);
                    mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeBaseWordsRevMixDictorPhrases);
                    mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideBaseWordsRevMixDictorPhrases);
                    mixParam.beforeAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeAllDirMixDictorPhrases);
                    mixParam.insideAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideAllDirMixDictorPhrases);
                    mixParam.beforeAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeAllRevMixDictorPhrases);
                    mixParam.insideAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideAllRevMixDictorPhrases);
                    mixParam.beforeFinishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeFinishDictorPhrases);
                    mixParam.finishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.finishDictorPhrases);
                }
            }

            return Ok(new { data, articleByParamDataWithAudio = articleByParamData });
        }

        //private async Task<IActionResult> GetByIdWithReadyAudioExec2(int id, SpaAppSetting spaAppSetting)
        //{
        //    var query = _context.ArticleByParams.Where(x => x.ArticleByParamID == id);

        //    var data = await query.FirstOrDefaultAsync();
        //    var lst = new List<APhWr>();
        //    var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(data.DataJson);
        //    if (articleByParamData != null)
        //    {
        //        var rootPath = _hostingEnvironment.WebRootPath;
        //        foreach (var mixParam in articleByParamData.MixParamsList)
        //        {
        //            mixParam.firstDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.firstDictorPhrases, true);
        //            mixParam.firstBeforeDialogDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.firstBeforeDialogDictorPhrases, true);
        //            mixParam.beforeByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeByOrderMixDictorPhrases, true);
        //            mixParam.insideByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.insideByOrderMixDictorPhrases, true);
        //            mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeBaseWordsDirMixDictorPhrases, true);
        //            mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.insideBaseWordsDirMixDictorPhrases, true);
        //            mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeBaseWordsRevMixDictorPhrases, true);
        //            mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.insideBaseWordsRevMixDictorPhrases, true);
        //            mixParam.beforeAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeAllDirMixDictorPhrases, true);
        //            mixParam.insideAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.insideAllDirMixDictorPhrases, true);
        //            mixParam.beforeAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeAllRevMixDictorPhrases, true);
        //            mixParam.insideAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.insideAllRevMixDictorPhrases, true);
        //            mixParam.beforeFinishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.beforeFinishDictorPhrases, true);
        //            mixParam.finishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
        //                        _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
        //                        mixParam.finishDictorPhrases, true);

        //            mixParam.MoveToWrappers();
        //            lst.AddRange(mixParam.tmpList);
        //            mixParam.firstDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.firstBeforeDialogDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeByOrderMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.insideByOrderMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeAllDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.insideAllDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeAllRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.insideAllRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.beforeFinishDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
        //            mixParam.finishDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();


        //            mixParam.firstDictorPhrases = "";
        //            mixParam.firstBeforeDialogDictorPhrases = "";
        //            mixParam.beforeByOrderMixDictorPhrases = "";
        //            mixParam.insideByOrderMixDictorPhrases = "";
        //            mixParam.beforeBaseWordsDirMixDictorPhrases = "";
        //            mixParam.insideBaseWordsDirMixDictorPhrases = "";
        //            mixParam.beforeBaseWordsRevMixDictorPhrases = "";
        //            mixParam.insideBaseWordsRevMixDictorPhrases = "";
        //            mixParam.beforeAllDirMixDictorPhrases = "";
        //            mixParam.insideAllDirMixDictorPhrases = "";
        //            mixParam.beforeAllRevMixDictorPhrases = "";
        //            mixParam.insideAllRevMixDictorPhrases = "";
        //            mixParam.beforeFinishDictorPhrases = "";
        //            mixParam.finishDictorPhrases = "";
        //        }

        //        articleByParamData.SelectedWords = new List<WordSelected>();
        //        articleByParamData.WordPhrasesToRepeat = new List<WordPhrase>();
        //        articleByParamData.WordsToRepeat = new List<Word>();
        //    }
        //    var ll = lst.Select(x => x.TFName).Distinct().ToList();
        //    return Ok(new { articleByParamDataWithAudio = articleByParamData });
        //}

        private async Task<ArticleByParamData> GetDataWithReadyAudio(ArticleByParam articleByParam, SpaAppSetting spaAppSetting)
        {
            var lst = new List<APhWr>();
            var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(articleByParam.DataJson);
            if (articleByParamData != null)
            {
                var rootPath = _hostingEnvironment.WebRootPath;
                foreach (var mixParam in articleByParamData.MixParamsList)
                {
                    mixParam.firstDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.firstDictorPhrases, true);
                    mixParam.firstBeforeDialogDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.firstBeforeDialogDictorPhrases, true);
                    mixParam.beforeByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeByOrderMixDictorPhrases, true);
                    mixParam.insideByOrderMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideByOrderMixDictorPhrases, true);
                    mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeBaseWordsDirMixDictorPhrases, true);
                    mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideBaseWordsDirMixDictorPhrases, true);
                    mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeBaseWordsRevMixDictorPhrases, true);
                    mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideBaseWordsRevMixDictorPhrases, true);
                    mixParam.beforeAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeAllDirMixDictorPhrases, true);
                    mixParam.insideAllDirMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideAllDirMixDictorPhrases, true);
                    mixParam.beforeAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeAllRevMixDictorPhrases, true);
                    mixParam.insideAllRevMixDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.insideAllRevMixDictorPhrases, true);
                    mixParam.beforeFinishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.beforeFinishDictorPhrases, true);
                    mixParam.finishDictorPhrasesWithAudio = AudioHelper.UpdateDictorAudio(
                                _context, _configuration, rootPath, spaAppSetting.NativeLanguage,
                                mixParam.finishDictorPhrases, true);

                    mixParam.MoveToWrappers();
                    lst.AddRange(mixParam.tmpList);
                    mixParam.firstDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.firstBeforeDialogDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeByOrderMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.insideByOrderMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeBaseWordsDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.insideBaseWordsDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeBaseWordsRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.insideBaseWordsRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeAllDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.insideAllDirMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeAllRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.insideAllRevMixDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.beforeFinishDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();
                    mixParam.finishDictorPhrasesWithAudio = new List<List<List<ArticlePhrase>>>();


                    mixParam.firstDictorPhrases = "";
                    mixParam.firstBeforeDialogDictorPhrases = "";
                    mixParam.beforeByOrderMixDictorPhrases = "";
                    mixParam.insideByOrderMixDictorPhrases = "";
                    mixParam.beforeBaseWordsDirMixDictorPhrases = "";
                    mixParam.insideBaseWordsDirMixDictorPhrases = "";
                    mixParam.beforeBaseWordsRevMixDictorPhrases = "";
                    mixParam.insideBaseWordsRevMixDictorPhrases = "";
                    mixParam.beforeAllDirMixDictorPhrases = "";
                    mixParam.insideAllDirMixDictorPhrases = "";
                    mixParam.beforeAllRevMixDictorPhrases = "";
                    mixParam.insideAllRevMixDictorPhrases = "";
                    mixParam.beforeFinishDictorPhrases = "";
                    mixParam.finishDictorPhrases = "";
                }

                articleByParamData.SelectedWords = new List<WordSelected>();
                articleByParamData.WordPhrasesToRepeat = new List<WordPhrase>();
                articleByParamData.WordsToRepeat = new List<Word>();
            }

            return  articleByParamData;
        }


        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] ArticleByParam articleByParam)
        {
            ArticleByParam oldItem;
            if (articleByParam.ArticleByParamID == 0)
            {
                oldItem = new ArticleByParam
                {
                    UserID = articleByParam.UserID,
                    NativeLanguageID = articleByParam.NativeLanguageID,
                    LearnLanguageID = articleByParam.LearnLanguageID,
                    Type = articleByParam.Type,
                    Name = articleByParam.Name,
                    DataJson = articleByParam.DataJson,
                    IsShared = articleByParam.IsShared,
                    IsTemplate = articleByParam.IsTemplate
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
                    oldItem = await ent.ArticleByParams.AsNoTracking()
                        .FirstOrDefaultAsync(x => x.ArticleByParamID == articleByParam.ArticleByParamID);
                }
            }

            if (
                oldItem.Name != articleByParam.Name
                || oldItem.DataJson != articleByParam.DataJson
                || oldItem.IsShared != articleByParam.IsShared
                || oldItem.IsTemplate != articleByParam.IsTemplate
                )
            {
                oldItem.Name = articleByParam.Name;
                oldItem.DataJson = articleByParam.DataJson;
                oldItem.IsShared = articleByParam.IsShared;
                oldItem.IsTemplate = articleByParam.IsTemplate;
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    ent.Update(oldItem);
                    ent.SaveChanges();
                }
            }

            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);

            var artParamsAfterCount = _context.ArticleByParams.AsNoTracking()
                    .Where(x =>
                        x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                        && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                        && x.UserID == userId
                        && x.ArticleByParamID > oldItem.ArticleByParamID
                        ).Count();

            if (artParamsAfterCount == 0)
            {
                // this item is last for this user in languages.
                // we can mark frases for repetition

                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(oldItem.DataJson);

                var curLessonNumber = 1;

                var query1 = _context.WordUsers.AsNoTracking()
                        .Where(x =>
                            x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                            && x.UserID == userId
                            && x.LastRepeatInArticleByParamID < oldItem.ArticleByParamID
                            );

                if (query1.Count() > 0)
                {
                    curLessonNumber = query1.Max(x => x.LastRepeatInLessonNum) + 1;
                }

                MarkRepeatedWordsAndPhrases(oldItem, articleByParamData, curLessonNumber, userId, spaAppSetting);
            }

            return Ok(oldItem);
        }

        private void MarkRepeatedWordsAndPhrases(ArticleByParam oldItem, ArticleByParamData articleByParamData, int curLessonNumber, Guid userId, SpaAppSetting spaAppSetting)
        {
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                var wordUserIds = new List<int>();
                // mark repetition phrases as repeated
                if (articleByParamData.WordPhrasesToRepeat != null)
                {
                    foreach (var wordPhrase in articleByParamData.WordPhrasesToRepeat)
                    {
                        var wordUser = ent.WordUsers.FirstOrDefault(x =>
                            x.WordID == wordPhrase.CurrentWordID
                            && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                            && x.UserID == userId
                            );

                        if (wordUser == null)
                        {
                            wordUser = new WordUser
                            { 
                                WordID = wordPhrase.CurrentWordID,
                                LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                                UserID = userId,
                                Grade = 1,
                                LastRepeatWordPhraseId = wordPhrase.WordPhraseID,
                            };

                            ent.WordUsers.Add(wordUser);
                        }
                        else
                        {
                            // set for this user's word which phrase was repeated last
                            wordUser.LastRepeatWordPhraseId = wordPhrase.WordPhraseID;
                            ent.Update(wordUser);
                        }

                        ent.SaveChanges();

                        if (!wordUserIds.Contains(wordPhrase.CurrentWordID))
                        {
                            wordUserIds.Add(wordPhrase.CurrentWordID);
                        }
                    }
                }

                // mark word as repeated in this lesson
                if (articleByParamData.WordsToRepeat != null)
                {
                    foreach (var word in articleByParamData.WordsToRepeat)
                    {
                        var wordUser = ent.WordUsers.FirstOrDefault(x =>
                            x.WordID == word.WordID
                            && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                            && x.UserID == userId
                            );

                        if (wordUser == null)
                        {
                            wordUser = new WordUser
                            {
                                WordID = word.WordID,
                                LanguageID = spaAppSetting.LearnLanguage.LanguageID,
                                UserID = userId,
                                Grade = 1,
                                LastRepeatInArticleByParamID = oldItem.ArticleByParamID,
                                LastRepeatInLessonNum = curLessonNumber
                            };

                            ent.WordUsers.Add(wordUser);
                        }
                        else
                        {
                            // set for this user's word which phrase was repeated last
                            wordUser.LastRepeatInArticleByParamID = oldItem.ArticleByParamID;
                            wordUser.LastRepeatInLessonNum = curLessonNumber;

                            ent.Update(wordUser);
                        }

                        ent.SaveChanges();

                        if (!wordUserIds.Contains(word.WordID))
                        {
                            wordUserIds.Add(word.WordID);
                        }
                    }
                }

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
                                    x.WordID == phrase.CurrentWordID
                                    && x.WordPhraseID == phrase.WordPhraseID
                                );
                            if (wordPhraseWords.PhraseOrder <= 0)
                            {
                                wordPhraseWords.PhraseOrder = index + 1;
                                index++;

                                ent.Update(wordPhraseWords);
                                ent.SaveChanges();
                            }
                        }
                    }
                }

                // save history of repetition mark changes
                var wordUsers = ent.WordUsers.Where(x =>
                    wordUserIds.Contains(x.WordID)
                    && x.LanguageID == spaAppSetting.LearnLanguage.LanguageID
                    && x.UserID == userId
                    ).ToList();

                foreach (var wordUser in wordUsers)
                {
                    var history = new List<RepeatHistory>();
                    if (!string.IsNullOrWhiteSpace(wordUser.RepeatHistory))
                    {
                        history = JsonConvert.DeserializeObject<List<RepeatHistory>>(wordUser.RepeatHistory);
                    }

                    history.Add( new RepeatHistory()
                    { 
                        Pr = wordUser.LastRepeatInArticleByParamID,
                        ln = wordUser.LastRepeatInLessonNum,
                        ph = wordUser.LastRepeatWordPhraseId
                    });

                    wordUser.RepeatHistory = JsonConvert.SerializeObject(history,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                    ent.Update(wordUser);
                    ent.SaveChanges();
                }
            }
        }

        public class RepeatHistory
        {
            public int Pr { get; set; }
            public int ln { get; set; }
            public int ph { get; set; }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] ArticleByParam articleByParam)
        {
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                // clear history in user words
                var wordUsers = ent.WordUsers
                    .Where(x => x.LastRepeatInArticleByParamID == articleByParam.ArticleByParamID)
                    .ToList();

                foreach (var wordUser in wordUsers)
                {
                    var history = new List<RepeatHistory>();
                    if (!string.IsNullOrWhiteSpace(wordUser.RepeatHistory))
                    {
                        history = JsonConvert.DeserializeObject<List<RepeatHistory>>(wordUser.RepeatHistory);
                    }

                    history = history.Where(x => x.Pr != articleByParam.ArticleByParamID)
                        .ToList();

                    wordUser.RepeatHistory = JsonConvert.SerializeObject(history,
                        new JsonSerializerSettings
                        {
                            ContractResolver = new CamelCasePropertyNamesContractResolver()
                        });

                    ent.Update(wordUser);
                    ent.SaveChanges();
                }

                // delete

                var oldItem = await ent.ArticleByParams.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleByParamID == articleByParam.ArticleByParamID);
                ent.Remove(oldItem);
                ent.SaveChanges();
            }

            return Ok();
        }

        [HttpGet]
        [ActionName("GetFiltered")]
        public async Task<IActionResult> GetFiltered([FromQuery] SearchListArg args)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var baseNameFilter = "";
            if (!string.IsNullOrEmpty(args.str1) && args.str1.IndexOf('_') > 0)
            {
                baseNameFilter = args.str1.Split('_')[0];
            }

            var query = _context.ArticleByParams.AsNoTracking()
                .Where(x => x.LearnLanguageID == spaAppSetting.LearnLanguage.LanguageID
                && x.NativeLanguageID == spaAppSetting.NativeLanguage.LanguageID
                && (x.UserID == userId || x.IsShared)
                && x.Type == args.int1
                && (baseNameFilter == "" || x.Name.StartsWith(baseNameFilter))
                )
                .Select(x => x);

            if (!string.IsNullOrEmpty(args.searchText))
            {
                query = query.Where(x => x.Name.Contains(args.searchText));
            }

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "ArticleByParamID";
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


            // make query
            var result = new ListResult<ArticleByParam>()
            {
                Count = await query.CountAsync(),
                Data = await query.Skip((args.Page) * args.PageSize).Take(args.PageSize).ToListAsync()
            };

            return Ok(result);
        }

        [HttpPost]
        [ActionName("Copy")]
        public async Task<IActionResult> Copy([FromBody] ArticleByParam articleByParam)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var userId = new Guid(user.Id);

            var oldItem = await _context.ArticleByParams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ArticleByParamID == articleByParam.ArticleByParamID);

            var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(oldItem.DataJson);
            articleByParamData.WordPhrasesToRepeat = new System.Collections.Generic.List<WordPhrase>();
            articleByParamData.WordsToRepeat = new System.Collections.Generic.List<Word>();
            articleByParamData.SelectedWords = new System.Collections.Generic.List<WordSelected>();

            var dataJson = JsonConvert.SerializeObject(articleByParamData,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                });

            var newItem = new ArticleByParam()
            {
                DataJson = dataJson,
                LearnLanguageID = oldItem.LearnLanguageID,
                Name = oldItem.Name + "_COPY_" + Guid.NewGuid().ToString(),
                NativeLanguageID = oldItem.NativeLanguageID,
                Type = oldItem.Type,
                UserID = userId
            };

            _context.Add(newItem);
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        [ActionName("Generate")]
        public async Task<IActionResult> Generate([FromBody] ArtParamsGenerationArg args)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid(user.Id);

            var oldItem = await _context.ArticleByParams.AsNoTracking()
                .FirstOrDefaultAsync(x => x.ArticleByParamID == args.ArticleByParamID);

            //var startWordRate = 1;
            //var startLessonNum = 1;
            //var endLessonNum = 11;
            //var wordsByLesson = 5;
            //var baseName = "en_test_lesson";
            //var maxWordsForRepetition = 25;

            var query =
                from w in _context.Words.AsNoTracking().Where(x => x.LanguageID == spaAppSetting.LearnLanguage.LanguageID)
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
                    WordID = w.WordID,
                    WordTranslation = wtj,
                    WordUser = wuj,
                };

            var words = query
                .Where(x => x.Rate >= args.startWordRate)
                .OrderBy(x => x.Rate)
                .Take(args.wordsByLesson * (args.endLessonNum - args.startLessonNum + 1))
                .ToList();

            // verify if already exists

            // with this name
            for (int currLessonNum = args.startLessonNum; currLessonNum <= args.endLessonNum; currLessonNum++)
            {
                var name = $"{args.baseName}-{currLessonNum.ToString("#0000")}";
                var exists = _context.ArticleByParams.Any(x =>
                    x.LearnLanguageID == oldItem.LearnLanguageID
                    && x.Name == name
                    && x.NativeLanguageID == oldItem.NativeLanguageID
                    && x.UserID == userId
                );

                if (exists)
                {
                    throw new Exception($"ArticleByParams name ({name}) already exists");
                }
            }

            var articleByParams = _context.ArticleByParams.Where(x =>
                    x.LearnLanguageID == oldItem.LearnLanguageID
                    && x.Name.StartsWith(args.baseName)
                    && x.NativeLanguageID == oldItem.NativeLanguageID
                    && x.UserID == userId
                );

            var wordIds = words.Select(x => x.WordID);
            foreach (var item in articleByParams)
            {
                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(item.DataJson);
                var exists = articleByParamData.SelectedWords.Where(x => wordIds.Contains(x.Word.WordID));
                if (exists.Count() > 0)
                {
                    throw new Exception($"word {exists.First().Word.Text} / already exists in {item.Name}");
                }
            }

            _wordManager.FillWordListWithPhrases(words, spaAppSetting, false);

            var wordIndes = 0;
            var lessonIndex = 0;
            for (int currLessonNum = args.startLessonNum; currLessonNum <= args.endLessonNum; currLessonNum++)
            {
                lessonIndex++;
                var articleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(oldItem.DataJson);
                articleByParamData.WordPhrasesToRepeat = new System.Collections.Generic.List<WordPhrase>();
                articleByParamData.WordsToRepeat = new System.Collections.Generic.List<Word>();
                articleByParamData.SelectedWords = new System.Collections.Generic.List<WordSelected>();
                
                articleByParamData.MaxWordsToRepeatForGeneration = args.maxWordsForRepetition;
                articleByParamData.BaseName = $"{args.baseName}-{currLessonNum.ToString("#0000")}";
                articleByParamData.FirstDictorPhrases = $"Урок {currLessonNum.ToString()}";
                articleByParamData.DialogText = "";
                articleByParamData.BeforeFinishDictorPhrases = "-";
                articleByParamData.FinishDictorPhrases = "-"; 

                // add words
                for (int i = wordIndes; i < (args.wordsByLesson * lessonIndex); i++)
                {
                    wordIndes++;
                    articleByParamData.SelectedWords.Add(
                        new WordSelected()
                        { 
                             Word = words[i],
                             PhraseWordsSelected = words[i].WordPhraseSelected.Take(3).ToList(),
                             PhraseWords = new System.Collections.Generic.List<WordPhrase>()
                        });
                }

                // add words for repetition


                var dataJson = JsonConvert.SerializeObject(articleByParamData,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                var newItem = new ArticleByParam()
                {
                    DataJson = dataJson,
                    LearnLanguageID = oldItem.LearnLanguageID,
                    Name = $"{args.baseName}-{currLessonNum.ToString("#0000")}",
                    NativeLanguageID = oldItem.NativeLanguageID,
                    Type = oldItem.Type,
                    UserID = userId,
                    IsShared = false,
                    IsTemplate = false
                };

                _context.Add(newItem);
                _context.SaveChanges();

                // generate words for repetition
                var forRepeat = await _wordManager.GetListForRepetition(spaAppSetting, userId, newItem.ArticleByParamID, args.baseName, articleByParamData.MaxWordsToRepeatForGeneration.Value);

                articleByParamData.WordsToRepeat = forRepeat;
                foreach (var item in forRepeat)
                {
                    articleByParamData.WordPhrasesToRepeat.AddRange(item.WordPhraseSelected);
                }
                
                dataJson = JsonConvert.SerializeObject(articleByParamData,
                    new JsonSerializerSettings
                    {
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    });

                newItem.DataJson = dataJson;
                _context.Update(newItem);
                _context.SaveChanges();

                MarkRepeatedWordsAndPhrases(newItem, articleByParamData, currLessonNum, userId, spaAppSetting);
            }

            return Ok();
        }


        [HttpGet]
        [ActionName("GetSchemas")]
        [Authorize(AuthenticationSchemes = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> GetSchemas([FromQuery] int version)
        {
            var userId = new Guid((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var curVersion = 4;

            if (curVersion == version)
                return Ok(new { version = curVersion, schemas = new List<object>() });

            var names = new List<string>
            { 
                "en_mobile_normal_template",
                "en_mobile_short_template",
                "en_mobile_huge_template",
                "test_tmp_short",
                "test_tmp_full"
            };

            var schemas = new List<ArticleByParamData>();
            var articleByParams = await _context.ArticleByParams.AsNoTracking()
                .Where(x => names.Contains(x.Name))
                .ToListAsync();

            foreach (var name in names)
            {
                var articleByParam = articleByParams.FirstOrDefault(x=>x.Name == name);
                var schema = await GetDataWithReadyAudio(articleByParam, spaAppSetting);
                schemas.Add(schema);
            }

            return Ok(new { version = curVersion, schemas = schemas });
        }
    }
}


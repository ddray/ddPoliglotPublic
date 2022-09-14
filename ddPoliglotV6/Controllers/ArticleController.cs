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
using Microsoft.WindowsAzure.Storage;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using Microsoft.AspNetCore.Authorization;
using ddPoliglotV6.BL.Managers;
using ddPoliglotV6.BL.Extentions;
using ddPoliglotV6.BL.Models;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using ddPoliglotV6.BL.Helpers;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.AspNetCore.Identity;

namespace ddPoliglotV6.Controllers
{
    //[Authorize(Roles = "LessonsMaker, Admin, SuperAdmin")]
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        ArticleManager _articleManager;
        private readonly UserManager<ApplicationUser> UserManager;

        public ArticleController(ddPoliglotDbContext context,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager
            )
        {
            context.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
           _articleManager = new ArticleManager(_context, _configuration);
            this.UserManager = userManager;
        }

        [HttpGet]
        [ActionName("GetTick")]
        public async Task<IActionResult> GetTick()
        {
            return Ok("{\"api_version\": \"v5.000004\"}");
        }

        [HttpGet]
        [ActionName("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] ListArg args)
        {
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid ((await this.UserManager.FindByNameAsync(User?.Identity?.Name)).Id);

            if (spaAppSetting?.LearnLanguage == null 
                || spaAppSetting?.NativeLanguage == null 
                || userId == null)
            {
                return Ok(new ListResult<Article>() { Count = 0, Data = new List<Article>() });
            }

            // prepare sort
            if (string.IsNullOrEmpty(args.Sort))
            {
                args.Sort = "ArticleID";
            }
            else
            {
                // first letter to upper case
                args.Sort = args.Sort.First().ToString().ToUpper() + args.Sort.Substring(1);
            }

            IQueryable<Article> query = _context.Articles.AsNoTracking().Where(x => x.UserID == userId 
                && x.Language == spaAppSetting.LearnLanguage.Code
                && x.LanguageTranslation == spaAppSetting.NativeLanguage.Code);

            if (args.Order == "desc")
            {
                query = query.OrderByDescending(p => EF.Property<object>(p, args.Sort));
            }
            else
            {
                query = query.OrderBy(p => EF.Property<object>(p, args.Sort));
            }


            var result = new ListResult<Article>()
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
            var result = await _articleManager.GetFullById(id);
            return Ok(result);
        }

        [HttpPost]
        [ActionName("Save")]
        public async Task<IActionResult> Save([FromBody] Article article)
        {
            try
            {
                Article oldArticle;
                if (article.ArticleID == 0)
                {
                    oldArticle = new Article
                    {
                        Language = article.Language,
                        LanguageTranslation = article.LanguageTranslation,
                        Name = article.Name.Trim(),
                        UserID = article.UserID,
                        ArticleActors = new List<ArticleActor>(),
                        ArticlePhrases = new List<ArticlePhrase>(),
                    };

                    using (var ent = new ddPoliglotDbContext(_configuration)) {
                        ent.Update(oldArticle);
                        ent.SaveChanges();
                    }

                    article.ArticleID = oldArticle.ArticleID;
                    foreach (var articlePhrase in article.ArticlePhrases)
                    {
                        articlePhrase.ArticleID = article.ArticleID;
                    }

                    foreach (var articleActor in article.ArticleActors)
                    {
                        articleActor.ArticleID = article.ArticleID;
                    }
                }
                else
                {
                    // get old article version
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        oldArticle = ent.Articles.Where(x => x.ArticleID == article.ArticleID).FirstOrDefault();
                    }
                }

                // delete deleted articlePhrases
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    // delete deleted ArticlePhrases
                    var aphIds = article.ArticlePhrases.Where(z => z.ArticlePhraseID > 0).Select(x => x.ArticlePhraseID).ToList();
                    var deletedArticlePhrases = ent.ArticlePhrases.Where(x => !aphIds.Contains(x.ArticlePhraseID) && x.ArticleID == oldArticle.ArticleID).ToList();

                    // first try to delete children if exist
                    var keys = deletedArticlePhrases.Select(x => x.KeyGuid.ToString()).ToList();
                    var deletedArticlePhrasesChildren = ent.ArticlePhrases.Where(x => keys.Contains(x.ParentKeyGuid) && x.ArticleID == oldArticle.ArticleID).ToList();

                    deletedArticlePhrases.AddRange(deletedArticlePhrasesChildren);
                    deletedArticlePhrases.GroupBy(x => x.KeyGuid).Select(y => y.FirstOrDefault()).ToList();

                    // delete mixParams for deleted phrases
                    foreach (var aPhrase in deletedArticlePhrases)
                    {
                        var mixParams = _context.MixParams.Where(x => x.ArticlePhraseKeyGuid == aPhrase.KeyGuid.ToString()).AsNoTracking().ToList();
                        foreach (var item in mixParams)
                        {
                            _context.Database.ExecuteSqlRaw($"delete from MixItems where MixParamID = {item.MixParamID}");
                            _context.Database.ExecuteSqlRaw($"delete from MixParams where MixParamID = {item.MixParamID}");
                        }
                    }

                    if (deletedArticlePhrases.Count > 0)
                    {
                        ent.RemoveRange(deletedArticlePhrases);
                        ent.SaveChanges();
                    }
                }
                
                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    // delete actors
                    var actorIds = article.ArticleActors.Where(z => z.ArticleActorID > 0).Select(x => x.ArticleActorID).ToList();
                    var deletedArticleActors = ent.ArticleActors.Where(x => !actorIds.Contains(x.ArticleActorID) && x.ArticleID == oldArticle.ArticleID).ToList();
                    if (deletedArticleActors.Count > 0)
                    {
                        ent.RemoveRange(deletedArticleActors);
                        ent.SaveChanges();
                    }

                    // Add new Actors
                    var newActors = article.ArticleActors.Where(x => x.ArticleActorID == 0).ToList();
                    if (newActors.Count > 0)
                    {
                        foreach (var item in newActors)
                        {
                            item.Article = null;
                        }

                        ent.AddRange(newActors);
                        ent.SaveChanges();
                    }

                    var oldActors = ent.ArticleActors.Where(x => x.ArticleID == oldArticle.ArticleID).ToList();

                    // changed Actors
                    foreach (var actor in article.ArticleActors)
                    {
                        var oldActor = oldActors.Where(y => y.ArticleActorID == actor.ArticleActorID).FirstOrDefault();
                        if (oldActor.DefaultInRole != actor.DefaultInRole
                            || oldActor.Name != actor.Name
                            || oldActor.VoicePitch != actor.VoicePitch
                            || oldActor.VoiceSpeed != actor.VoiceSpeed
                            || oldActor.VoiceName != actor.VoiceName
                            )
                        {
                            oldActor.DefaultInRole = actor.DefaultInRole;
                            oldActor.Name = actor.Name;
                            oldActor.VoicePitch = actor.VoicePitch;
                            oldActor.VoiceSpeed = actor.VoiceSpeed;
                            oldActor.VoiceName = actor.VoiceName;

                            ent.Update(oldActor);
                            ent.SaveChanges();
                        }
                    }

                    // set new actor IDs for phrases
                    foreach (var articlePhrase in article.ArticlePhrases)
                    {
                        if (articlePhrase.ArticleActorID == 0)
                        {
                            var actor = oldActors.FirstOrDefault(x => x.KeyGuid == articlePhrase.ArticleActor.KeyGuid);
                            articlePhrase.ArticleActorID = actor.ArticleActorID;
                        }

                        if (articlePhrase.TrArticleActorID == 0)
                        {
                            var actor = oldActors.FirstOrDefault(x => x.KeyGuid == articlePhrase.TrArticleActor.KeyGuid);
                            articlePhrase.TrArticleActorID = actor.ArticleActorID;
                        }
                    }
                }

                // trim texts and recalculate HashCode for articles which already has audio
                foreach (var articlePhrase in article.ArticlePhrases)
                {
                    articlePhrase.Text = ArticleManager.TrimEx(articlePhrase.Text, articlePhrase);

                    if (!string.IsNullOrEmpty(articlePhrase.TextSpeechFileName))
                    {
                        var oldHashCode = articlePhrase.HashCode;
                        articlePhrase.HashCode = articlePhrase.GetPhraseHash();
                        if (articlePhrase.HashCode != oldHashCode)
                        {
                            articlePhrase.TextSpeechFileName = "";
                        }
                    }

                    articlePhrase.TrText = ArticleManager.TrimEx(articlePhrase.TrText, articlePhrase);

                    if (!string.IsNullOrEmpty(articlePhrase.TrTextSpeechFileName))
                    {
                        var oldHashCode = articlePhrase.TrHashCode;
                        articlePhrase.TrHashCode = articlePhrase.GetTrPhraseHash();
                        if (articlePhrase.TrHashCode != oldHashCode)
                        {
                            articlePhrase.TrTextSpeechFileName = "";
                        }
                    }
                }

                using (var ent = new ddPoliglotDbContext(_configuration))
                {
                    // add new ArticlePhrases
                    var newArticlePhrases = article.ArticlePhrases.Where(x => x.ArticlePhraseID == 0).ToList();
                    if (newArticlePhrases.Count > 0)
                    {
                        foreach (var item in newArticlePhrases)
                        {
                            item.Article = null;
                            item.ArticleActor = null;
                            item.TrArticleActor = null;
                        }

                        ent.UpdateRange(newArticlePhrases);
                        ent.SaveChanges();
                    }
  
                    var oldArticlePhrases = ent.ArticlePhrases.Where(x => x.ArticleID == oldArticle.ArticleID).ToList();

                    // changed articlePhrases
                    foreach (var articlePhrase in article.ArticlePhrases.ToList())
                    {
                        var oldArticlePhrase = oldArticlePhrases.Where(y => y.ArticlePhraseID == articlePhrase.ArticlePhraseID).FirstOrDefault();
                        if (oldArticlePhrase != null)
                        {
                            if (oldArticlePhrase.OrderNum != articlePhrase.OrderNum
                                || oldArticlePhrase.ActivityType != articlePhrase.ActivityType
                                || oldArticlePhrase.ArticleActorID != articlePhrase.ArticleActorID
                                || oldArticlePhrase.OrderNum != articlePhrase.OrderNum
                                || oldArticlePhrase.Pause != articlePhrase.Pause
                                || oldArticlePhrase.SpeachDuration != articlePhrase.SpeachDuration
                                || oldArticlePhrase.Text != articlePhrase.Text
                                || oldArticlePhrase.TextSpeechFileName != articlePhrase.TextSpeechFileName
                                || oldArticlePhrase.TrArticleActorID != articlePhrase.TrArticleActorID
                                || oldArticlePhrase.TrHashCode != articlePhrase.TrHashCode
                                || oldArticlePhrase.TrPause != articlePhrase.TrPause
                                || oldArticlePhrase.TrSpeachDuration != articlePhrase.TrSpeachDuration
                                || oldArticlePhrase.TrText != articlePhrase.TrText
                                || oldArticlePhrase.TrTextSpeechFileName != articlePhrase.TrTextSpeechFileName
                                || oldArticlePhrase.Type != articlePhrase.Type
                                || oldArticlePhrase.ParentKeyGuid != articlePhrase.ParentKeyGuid
                                || oldArticlePhrase.HasChildren != articlePhrase.HasChildren
                                || oldArticlePhrase.ChildrenType != articlePhrase.ChildrenType
                                || oldArticlePhrase.ChildrenClosed != articlePhrase.ChildrenClosed
                                || oldArticlePhrase.Silent != articlePhrase.Silent

                                    )
                            {
                                oldArticlePhrase.ActivityType = articlePhrase.ActivityType;
                                oldArticlePhrase.ArticleActorID = articlePhrase.ArticleActorID;
                                oldArticlePhrase.OrderNum = articlePhrase.OrderNum;
                                oldArticlePhrase.Pause = articlePhrase.Pause;
                                oldArticlePhrase.SpeachDuration = articlePhrase.SpeachDuration;
                                oldArticlePhrase.Text = articlePhrase.Text;
                                oldArticlePhrase.TextSpeechFileName = articlePhrase.TextSpeechFileName;
                                oldArticlePhrase.TrArticleActorID = articlePhrase.TrArticleActorID;
                                oldArticlePhrase.TrHashCode = articlePhrase.TrHashCode;
                                oldArticlePhrase.TrPause = articlePhrase.TrPause;
                                oldArticlePhrase.TrSpeachDuration = articlePhrase.TrSpeachDuration;
                                oldArticlePhrase.TrText = articlePhrase.TrText;
                                oldArticlePhrase.TrTextSpeechFileName = articlePhrase.TrTextSpeechFileName;
                                oldArticlePhrase.Type = articlePhrase.Type;
                                oldArticlePhrase.ParentKeyGuid = articlePhrase.ParentKeyGuid;
                                oldArticlePhrase.HasChildren = articlePhrase.HasChildren;
                                oldArticlePhrase.ChildrenType = articlePhrase.ChildrenType;
                                oldArticlePhrase.Silent = articlePhrase.Silent;
                                // oldArticlePhrase.ChildrenClosed = articlePhrase.ChildrenClosed;
                                oldArticlePhrase.Article = null;
                                oldArticlePhrase.ArticleActor = null;
                                oldArticlePhrase.TrArticleActor = null;

                                ent.Update(oldArticlePhrase);
                                ent.SaveChanges();
                            }
                        }
                    }
                }

                if (
                    oldArticle.Name != article.Name
                    || oldArticle.Language != article.Language
                    || oldArticle.LanguageTranslation != article.LanguageTranslation
                    )
                {
                    oldArticle.Name = article.Name;
                    oldArticle.Language = article.Language;
                    oldArticle.LanguageTranslation = article.LanguageTranslation;
                    using (var ent = new ddPoliglotDbContext(_configuration))
                    {
                        ent.Update(oldArticle);
                        ent.SaveChanges();
                    }
                }

                return Ok(article.ArticleID);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [ActionName("Delete")]
        public async Task<IActionResult> Delete([FromBody] Article article)
        {
            try
            {
                var oldArticle = await _context.Articles.AsNoTracking().SingleAsync(x=>x.ArticleID == article.ArticleID);

                var articlePhrases = await _context.ArticlePhrases.Where(x => x.Article.ArticleID == article.ArticleID).ToListAsync();

                // delete mixParams for deleted phrases
                foreach (var aPhrase in articlePhrases)
                {
                    var mixParams = _context.MixParams.Where(x => x.ArticlePhraseKeyGuid == aPhrase.KeyGuid.ToString()).AsNoTracking().ToList();
                    foreach (var item in mixParams)
                    {
                        _context.Database.ExecuteSqlRaw($"delete from MixItems where MixParamID = {item.MixParamID}");
                        _context.Database.ExecuteSqlRaw($"delete from MixParams where MixParamID = {item.MixParamID}");
                    }
                }

                _context.RemoveRange(articlePhrases);
                await _context.SaveChangesAsync();

                var articleActors = await _context.ArticleActors.Where(x => x.Article.ArticleID == article.ArticleID).ToListAsync();
                _context.RemoveRange(articleActors);
                await _context.SaveChangesAsync();

                _context.Remove(oldArticle);
                await _context.SaveChangesAsync();

                return Ok();
        
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        [HttpPost]
        [ActionName("text_to_speach_article_phrase")]
        public async Task<IActionResult> TextToSpeachArticlePhrase([FromBody]TextToSpeechArticlePhraseArg args)
        {
            await _articleManager.TextToSpeachArticlePhrase(args.ArticlePhraseID.Value, _hostingEnvironment.WebRootPath);
            return Ok();
        }

        [HttpPost]
        [ActionName("textToSpeachArticle")]
        public async Task<IActionResult> TextToSpeachArticle([FromBody] TextToSpeechArticlePhraseArg args)
        {
            args.BaseRootPath = _hostingEnvironment.WebRootPath;
            await _articleManager.TextToSpeachArticleAsync(args);
            
            return Ok();
        }

        [HttpPost]
        [ActionName("text_to_video_article")]
        public async Task<IActionResult> TextToVideoArticle([FromBody]TextToSpeechArticlePhraseArg args)
        {
            args.BaseRootPath = _hostingEnvironment.WebRootPath;
            await _articleManager.TextToVideoArticle(args);
            return Ok();
        }

        [HttpPost]
        [ActionName("textToVideoArticleRetrialState")]
        public async Task<IActionResult> TextToVideoArticleRetrialState([FromBody]TextToSpeechArticlePhraseArg args)
        {
            // get state of video making process from table and send it to client
            var acc = new AzureAccess(_configuration["AccessKey"]);
            StorageCredentials creds = new StorageCredentials(acc.AccountName, acc.AccountKey);
            CloudStorageAccount account = string.IsNullOrEmpty(acc.TableEndpoint)
            ? new CloudStorageAccount(creds, useHttps: true)
            : new CloudStorageAccount(creds, null, null, new StorageUri(new Uri(acc.TableEndpoint)), null);

            CloudTableClient client = account.CreateCloudTableClient();
            CloudTable table = client.GetTableReference(_configuration["GenerateVideoForArticleStateTable"]);
            await table.CreateIfNotExistsAsync();
            var entity = await table.ExecuteAsync(TableOperation.Retrieve<WorkJobsStateEntity>("GenerateVideoForArticle", args.SessionID));
            return Ok(entity.Result);
        }

        [HttpPost]
        [ActionName("GenerateVideoAndAudio")]
        public async Task<IActionResult> GenerateFromArticles([FromBody] ArtParamsGenerationArg args)
        {
            var user = await this.UserManager.FindByNameAsync(User?.Identity?.Name);
            var spaAppSetting = new SpaAppSetting(HttpContext.Request.Headers, _context);
            var userId = new Guid(user.Id);

            List<Article> articles = null;
            using (var ent = new ddPoliglotDbContext(_configuration))
            {
                articles = ent.Articles.AsNoTracking()
                    .Where(x => x.Name.StartsWith(args.baseName))
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            var artDrs = new List<articleGr>();
            foreach (var article in articles)
            {
                var ar = article.Name.Split(args.baseName, StringSplitOptions.RemoveEmptyEntries)[0]
                    .Replace("-", "").Split("_", StringSplitOptions.RemoveEmptyEntries);
                artDrs.Add(new articleGr()
                {
                    Article = article,
                    Num = Convert.ToInt32(ar[0]),
                    Part = Convert.ToInt32(ar[1])
                });
            }

            var gr = from ag in artDrs
                     group ag by ag.Num into agGroup
                     where
                        agGroup.Key >= args.startLessonNum
                        && agGroup.Key <= args.endLessonNum
                     orderby agGroup.Key
                     select agGroup;

            // generate audio and video
            foreach (var items in gr)
            {
                var parts = items.OrderBy(x => x.Part).ToList();
                foreach (var part in parts)
                {
                    var article = part.Article;
                    if (string.IsNullOrWhiteSpace(article.TextSpeechFileName))
                    {
                        await _articleManager.TextToSpeachArticleAsync(new TextToSpeechArticlePhraseArg() 
                            {
                                ArticleID = part.Article.ArticleID,
                                BaseRootPath = _hostingEnvironment.WebRootPath,
                                SelectedArticlePhraseIDs = new List<int>(),
                            });
                    }

                    if (string.IsNullOrWhiteSpace(article.VideoFileName))
                    {
                        await _articleManager.TextToVideoArticle(new TextToSpeechArticlePhraseArg()
                        {
                            ArticleID = part.Article.ArticleID,
                            BaseRootPath = _hostingEnvironment.WebRootPath,
                            SelectedArticlePhraseIDs = new List<int>(),
                        });
                    }
                }
            }

            return Ok();
        }
    }
}


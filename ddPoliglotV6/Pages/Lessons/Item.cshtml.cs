using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.BL.Models;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.Infrastructure.Route;
using ddPoliglotV6.Infrastructure.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ddPoliglotV6.Pages.Lessons
{
    public class ItemModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ddPoliglotDbContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> UserManager;
        private readonly CommonLocalizationService _localizer;
        
        public ItemModel(
            ddPoliglotDbContext context,
            ILogger<IndexModel> logger,
            IHostingEnvironment hostingEnvironment,
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager,
            CommonLocalizationService localizer
            )
        {
            _logger = logger;
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            UserManager = userManager;
            _localizer = localizer;

            this.ImageBaseUrl = _configuration["lessonsImageUrl"];
            this.AudioBaseUrl = _configuration["lessonsAudioUrl"];
        }

        public Lesson Lesson { get; set; }
        public ArticleByParamData ArticleByParamData { get; set; }
        public string ImageBaseUrl { get; set; }
        public string AudioBaseUrl { get; set; }

        public async Task OnGetAsync()
        {
            var pageName = (RouteData.Values["lesson-page-alias"] ?? "").ToString();
            var learnLanguage = RoutersTree.GetLanguageByLessonsFolderAliasName((RouteData.Values["lessons-folder-alias"] ?? "").ToString());
            var pageLanguage = Enum.Parse(typeof(Languages), RouteData.Values["culture"].ToString());

            ViewData["ActiveMenu"] = learnLanguage == Languages.sk
                ? MenuItemName.LessonSk.ToString()
                : MenuItemName.LessonEn.ToString();

            ViewData["Text1"] = learnLanguage == Languages.sk
                ? _localizer.Get("xxFree Slovakian lesson")
                : _localizer.Get("xxFree English lesson");
            var languageId = (int)learnLanguage;
            var nativeLanguageId = (int)learnLanguage;


            Lesson = await _context.Lessons.AsNoTracking().FirstOrDefaultAsync(x => 
                x.PageName == pageName 
                && x.LanguageID == (int)learnLanguage
                && x.NativeLanguageID == (int)pageLanguage
            );

            if ((Lesson?.ArticleByParamID ?? 0) > 0)
            {
                Lesson.ArticleByParam = await _context.ArticleByParams.AsNoTracking()
                    .FirstOrDefaultAsync(x => x.ArticleByParamID == Lesson.ArticleByParamID);
                ArticleByParamData = JsonConvert.DeserializeObject<ArticleByParamData>(Lesson.ArticleByParam.DataJson);
                ViewData["Title"] = Lesson.PageMetaTitle;
                ViewData["Description"] = Lesson.PageMetaDescription;
            }
        }
    }
}
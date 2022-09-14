using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ddPoliglotV6.BL.Enums;
using ddPoliglotV6.Data;
using ddPoliglotV6.Data.Models;
using ddPoliglotV6.Infrastructure.Route;
using ddPoliglotV6.Infrastructure.Services;
using ddPoliglotV6.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace ddPoliglotV6.Pages.Lessons
{
    public class ListModel : PageModel
    {
        private readonly ddPoliglotDbContext _context;
        private readonly CommonLocalizationService _localizer;

        public ListModel(ddPoliglotDbContext context, CommonLocalizationService localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public PaginatedList<Lesson> Items { get; set; }

        public async Task OnGetAsync(int? pageIndex)
        {
            ViewData["lessons-folder-alias"] = (RouteData.Values["lessons-folder-alias"] ?? "").ToString();
            var learnLanguage = RoutersTree.GetLanguageByLessonsFolderAliasName((RouteData.Values["lessons-folder-alias"] ?? "").ToString());
            var pageLanguage = Enum.Parse(typeof(Languages), RouteData.Values["culture"].ToString());

            if (learnLanguage == Languages.en)
            {
                ViewData["Title"] = _localizer.Get("pageTitleLessonsEn");
                ViewData["Description"] = _localizer.Get("pageDescriptionLessonsEn");
            }
            else if (learnLanguage == Languages.sk)
            {
                ViewData["Title"] = _localizer.Get("pageTitleLessonsSk");
                ViewData["Description"] = _localizer.Get("pageDescriptionLessonsSk");
            }
            else
            {
                ViewData["Title"] = _localizer.Get("pageTitleLessonsEn");
                ViewData["Description"] = _localizer.Get("pageDescriptionLessonsEn");
            }

            ViewData["ActiveMenu"] = learnLanguage == Languages.sk
                ? MenuItemName.LessonSk.ToString()
                : MenuItemName.LessonEn.ToString();

            var cont = RouteData.Values;
            int pageSize = 20;

            var query = from l in _context.Lessons
                        where l.NativeLanguageID == (int)pageLanguage
                        && l.LanguageID == (int)learnLanguage
                        && l.ParentID == 0
                        select l;
            if ((pageIndex ?? 1) <= 0)
            {
                pageIndex = 1;
            }

            this.Items = await PaginatedList<Lesson>.CreateAsync(
                query.AsNoTracking(), pageIndex ?? 1, pageSize);
        }
    }
}
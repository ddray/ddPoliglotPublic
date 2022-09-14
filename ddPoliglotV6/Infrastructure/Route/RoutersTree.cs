using ddPoliglotV6.BL.Enums;
using System.Collections.Generic;
using System.Linq;

namespace ddPoliglotV6.Infrastructure.Route
{
    public class RoutersTree
    {
        #region Lessons list page

        private static List<RouteTreeItem> listLanguageLessons = new List<RouteTreeItem>()
        {
            new RouteTreeItem() { PageLanguage = Languages.ru, LearnLanguage=Languages.sk, LessonPage = "urok", LessonsPage="slovatskiy-jazik-bezplatnye-uroki" },
            new RouteTreeItem() { PageLanguage = Languages.ru, LearnLanguage=Languages.en, LessonPage = "urok", LessonsPage="anglijskij-jazik-bezplatnye-uroki" },
            new RouteTreeItem() { PageLanguage = Languages.en, LearnLanguage=Languages.sk, LessonPage = "lesson", LessonsPage="slovakian-language-free-lessons" },
            new RouteTreeItem() { PageLanguage = Languages.en, LearnLanguage=Languages.en, LessonPage = "lesson", LessonsPage="english-language-free-lessons" },
            new RouteTreeItem() { PageLanguage = Languages.sk, LearnLanguage=Languages.sk, LessonPage = "urok", LessonsPage="slovencina-bezplatne-uroky" },
            new RouteTreeItem() { PageLanguage = Languages.sk, LearnLanguage=Languages.en, LessonPage = "urok", LessonsPage="angliscina-bezplatne-uroky" },
        };

        public static List<RouteTreeItem> GetValues()
        {
            return new List<RouteTreeItem>();
        }

        public static List<string> GetLessonsFolderPagesNames()
        {
            return listLanguageLessons.Select(x => x.LessonsPage).ToList();
        }

        public static string GetLessonsFolderAliasName(string language, string learnLanguage)
        {
            return listLanguageLessons.Where(x => x.PageLanguage.ToString() == language && x.LearnLanguage.ToString() == learnLanguage).Select(x => x.LessonsPage).FirstOrDefault();
        }
        public static Languages GetLanguageByLessonsFolderAliasName(string aliasName)
        {
            return listLanguageLessons.Where(x => x.LessonPage == aliasName || x.LessonsPage == aliasName).Select(x => x.LearnLanguage).FirstOrDefault();
        }
        public static string ReplacePathLessonsGetNew(string aliasName, string currLanguage, string newLanguage)
        {
            if (string.IsNullOrEmpty(aliasName))
            {
                return "";
            }

            var learnLesson = GetLanguageByLessonsFolderAliasName(aliasName);
            return GetLessonsFolderAliasName(newLanguage, learnLesson.ToString());
        }

        #endregion Lessons list page
    }

    public class RouteTreeItem
    {
        public Languages PageLanguage { get; set; }
        public Languages LearnLanguage { get; set; }
        public string LessonsPage { get; set; }
        public string LessonPage { get; set; }
    }
}

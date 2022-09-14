using ddPoliglotV6.Data.Models;
using Google.Apis.YouTube.v3.Data;

namespace ddPoliglotV6.BL.Models
{
    public class ListArg
    {
        public string Sort { get; set; }
        public string Order { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class SearchListArg: ListArg
    {
        public int parentID { get; set; }
        public string searchText { get; set; }
        public int RateFrom { get; set; }
        public int RateTo { get; set; }
        public int GradeFrom { get; set; }
        public int GradeTo { get; set; }
        public bool Recomended { get; set; }
        public string str1 { get; set; }
        public string str2 { get; set; }
        public int int1 { get; set; }
        public int int2 { get; set; }
    }
    public class ArtParamsGenerationArg
    {
        public int ArticleByParamID { get; set; }
        public string baseName { get; set; }
        public int startWordRate { get; set; }
        public int startLessonNum { get; set; }
        public int endLessonNum { get; set; }
        public int wordsByLesson { get; set; }
        public int maxWordsForRepetition { get; set; }
        public string str1 { get; set; }
        public string str2 { get; set; }
        public int int1 { get; set; }
        public int int2 { get; set; }
    }

    public class articleGr
    {
        public Article Article { get; set; }
        public string BaseName { get; set; }
        public int Num { get; set; }
        public int Part { get; set; }
        public Video Video { get; set; }
    }
}

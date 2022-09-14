using System.Collections.Generic;

namespace ddPoliglotV6.BL.Models
{
    public class UserLessonArgs
    {
        public List<int> wordIDs { get; set; }
        public List<int> repeatWordIds { get; set; }
        public List<string> repeatWordPhrasesData { get; set; }
        public int lessonType { get; set; }
        public int lessonNum { get; set; }
    }
}

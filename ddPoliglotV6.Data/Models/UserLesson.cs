using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class UserLesson
    {
        [Key]
        public int UserLessonID { get; set; }
        public int LanguageID { get; set; }
        public Guid UserID { get; set; }
        public int Num { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public DateTime Finished { get; set; }
        public int TotalSeconds { get; set; }
        public int LearnSeconds { get; set; }
        public int LessonType { get; set; } // 0-normal, 1 - short, 2 - huge
        public List<UserLessonWord> UserLessonWords { get; set; }
    }
}

using System;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class UserLessonWord
    {
        [Key]
        public int UserLessonWordID { get; set; }
        public int UserLessonID { get; set; }
        public int WordID { get; set; }
        public Word Word { get; set; }
        public int wordType { get; set; } // 0 - main, 1 - in repetition list
    }
}

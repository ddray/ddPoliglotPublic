using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class WordUser
    {
        [Key]
        public int WordUserID { get; set; }

        public int WordID { get; set; }

        [ForeignKey("WordID")]
        public Word Word { get; set; }

        public Guid UserID { get; set; }

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; }

        public int Grade { get; set; }

        public int LastRepeatWordPhraseId { get; set; }

        public int LastRepeatInLessonNum { get; set; }

        public int LastRepeatInArticleByParamID { get; set; }

        public string RepeatHistory { get; set; }

        public int SourceType { get; set; } // 0 - set grade by hand, 1- by auto (language level)
    }
}

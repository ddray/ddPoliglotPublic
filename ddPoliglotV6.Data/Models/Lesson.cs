using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class Lesson
    {
        [Key]
        public int LessonID { get; set; }

        public int ParentID { get; set; } // Parent Lesson

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; } // language to learn

        public int NativeLanguageID { get; set; } // native user language

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string Title { get; set; }

        [MaxLength(3000)]
        public string Description { get; set; }

        public string Content { get; set; }

        public int Order { get; set; }

        [MaxLength(250)]
        public string Video1 { get; set; }

        [MaxLength(250)]
        public string Video2 { get; set; }

        [MaxLength(250)]
        public string Video3 { get; set; }

        [MaxLength(250)]
        public string Video4 { get; set; }

        [MaxLength(250)]
        public string Video5 { get; set; }

        [MaxLength(250)]
        public string Audio1 { get; set; }

        [MaxLength(250)]
        public string Audio2 { get; set; }

        [MaxLength(250)]
        public string Audio3 { get; set; }

        [MaxLength(250)]
        public string Audio4 { get; set; }

        [MaxLength(250)]
        public string Audio5 { get; set; }

        [MaxLength(250)]
        public string Image1 { get; set; }

        [MaxLength(250)]
        public string Image2 { get; set; }

        [MaxLength(250)]
        public string Image3 { get; set; }

        [MaxLength(250)]
        public string Image4 { get; set; }

        [MaxLength(250)]
        public string Image5 { get; set; }
        [MaxLength(3000)]
        public string Description1 { get; set; }

        [MaxLength(3000)]
        public string Description2 { get; set; }

        [MaxLength(3000)]
        public string Description3 { get; set; }

        [MaxLength(3000)]
        public string Description4 { get; set; }

        [MaxLength(3000)]
        public string Description5 { get; set; }

        public DateTime Modified { get; set; }

        [MaxLength(20)]
        public string State { get; set; }

        [MaxLength(20)]
        public string PageName { get; set; }
        public int ArticleByParamID { get; set; }

        [MaxLength(500)]
        public string PageMetaTitle { get; set; }

        [MaxLength(1000)]
        public string PageMetaDescription { get; set; }

        [NotMapped]
        public ArticleByParam ArticleByParam { get; set; }
    }
}

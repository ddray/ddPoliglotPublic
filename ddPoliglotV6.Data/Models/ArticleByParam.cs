
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class ArticleByParam
    {
        public int ArticleByParamID { get; set; }
        
        public Guid UserID { get; set; }

        public int NativeLanguageID { get; set; }

        public int LearnLanguageID { get; set; }

        public int Type { get; set; } // 0 - article by vocabulary, 1 - article by dialog

        [MaxLength(250)]
        public string Name { get; set; }

        public string DataJson { get; set; }

        public bool IsTemplate { get; set; }

        public bool IsShared { get; set; }
    }
}

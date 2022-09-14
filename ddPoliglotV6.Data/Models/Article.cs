
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ddPoliglotV6.Data.Models
{
    public class Article
    {
        public int ArticleID { get; set; }
        
        public Guid UserID { get; set; }
        
        [MaxLength(250)]
        public string Name { get; set; }
        
        [MaxLength(50)]
        public string Language { get; set; }
        
        [MaxLength(50)]
        public string LanguageTranslation { get; set; }
        
        [MaxLength(50)]
        public string TextHashCode { get; set; }
        
        [MaxLength(100)]
        public string TextSpeechFileName { get; set; }

        [MaxLength(100)]
        public string VideoFileName { get; set; }
        public ICollection<ArticlePhrase> ArticlePhrases { get; set; }
        
        public ICollection<ArticleActor> ArticleActors { get; set; }
    }
}

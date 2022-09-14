using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class ArticlePhrase
    {
        [Key]
        public int ArticlePhraseID { get; set; }
        public Guid KeyGuid { get; set; }

        public int ArticleID { get; set; }
        [ForeignKey("ArticleID")]
        public Article Article { get; set; }

        // Phrase
        [MaxLength(2000)]
        public string Text { get; set; }
        public Int64 HashCode { get; set; }
        [MaxLength(100)]
        public string TextSpeechFileName { get; set; }
        public int SpeachDuration { get; set; }
        public int Pause { get; set; } // how many additional seconds add after source audio

        public int ArticleActorID { get; set; }
        [ForeignKey("ArticleActorID")]
        public ArticleActor ArticleActor { get; set; }

        // Phrase translation
        [MaxLength(2000)]
        public string TrText { get; set; }
        public Int64 TrHashCode { get; set; }
        [MaxLength(200)]
        public string TrTextSpeechFileName { get; set; }
        public int TrSpeachDuration { get; set; }
        public int TrPause { get; set; } // how many additional seconds add after translation audio
        public int TrArticleActorID { get; set; }
        [ForeignKey("TrArticleActorID")]
        public ArticleActor TrArticleActor { get; set; }

        // props
        public int OrderNum { get; set; }
        public int ActivityType { get; set; } // phrase or translation is used in article, 0 = source first, 1 = translation first
        public int Type { get; set; } // 0 - text speach, 1 - Dictor Speach

        // grouping as child
        [MaxLength(50)]
        public string ParentKeyGuid { get; set; } 
        [MaxLength(50)]
        public string ChildrenType { get; set; } // 0 01 012 0123, if is empty - added by hand

        // as parent
        public bool HasChildren { get; set; }
        public bool ChildrenClosed { get; set; }

        [NotMapped]
        public bool Selected { get; set; }

        public bool Silent { get; set; } // don't reproduce this phrase

        public APhWr ToWrapper()
        {
            return new APhWr()
            {
                ID = ArticlePhraseID,
                SD = SpeachDuration,
                T = Text,
                TFName = TextSpeechFileName
            };
        }

    }
    public class APhWr
    {
        public int ID { get; set; }
        public string T { get; set; }
        public string TFName { get; set; }
        public int SD { get; set; }
    }
}

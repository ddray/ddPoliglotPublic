using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class WordPhraseWord
    {
        [Key]
        public int WordPhraseWordID { get; set; }

        public int WordID { get; set; }
        [ForeignKey("WordID")]
        public Word Word { get; set; }

        public int WordPhraseID { get; set; }
        [ForeignKey("WordPhraseID")]
        public WordPhrase WordPhrase { get; set; }

        public int PhraseOrder { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}

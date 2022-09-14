using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class WordsForPhraseState
    {
        [Key]
        public int WordsForPhraseStateID { get; set; }

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; } // language to learn

        public int NativeLanguageID { get; set; } // native user language

        public string WordsForPhraseStateData { get; set; } // JSon data
    }
}

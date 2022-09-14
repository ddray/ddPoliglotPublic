using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class DictionaryVersion
    {
        [Key]
        public int DictionaryVersionID { get; set; }

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; }

        public int NativeLanguageID { get; set; } // native user language

        public int Value { get; set; }

        [Column(TypeName = "text")]
        public string Description { get; set; }
    }
}

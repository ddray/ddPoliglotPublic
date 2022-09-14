using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class MixParamTextTemp
    {
        public int MixParamTextTempID { get; set; }
        [Column(TypeName = "ntext")]
        public string Text { get; set; }
        [MaxLength(100)]
        public string KeyTemp { get; set; }
        public int LearnLanguageID { get; set; }
        public int NativeLanguageID { get; set; }
    }
}

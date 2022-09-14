using System.Collections.Generic;

namespace ddPoliglotV6.BL.Models
{
    public class TranslateArrayArg
    {
        public List<string> Source { get; set; }
        public string SourceLanguage { get; set; }
        public string TargetLanguage { get; set; }
    }
}

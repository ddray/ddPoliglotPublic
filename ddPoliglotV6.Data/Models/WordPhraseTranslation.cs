using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class WordPhraseTranslation
    {
        [Key]
        public int WordPhraseTranslationID { get; set; }

        public int WordPhraseID { get; set; }

        [ForeignKey("WordPhraseID")]
        public WordPhrase WordPhrase { get; set; }

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; }

        [MaxLength(250)]
        public string Text { get; set; }

        public Int64 HashCode { get; set; }

        [MaxLength(100)]
        public string TextSpeechFileName { get; set; }

        public int SpeachDuration { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string Compare(WordPhraseTranslation other)
        {
            string[] propNames = new string[] {
                "LanguageID",
                "Text",
                "HashCode",
                "TextSpeechFileName",
                "SpeachDuration"
            };

            var result = new StringBuilder();
            foreach (var propName in propNames)
            {
                CompareProp(this, other, propName, result);
            }

            return result.ToString();
        }

        private void CompareProp(object curr, object other, string propName, StringBuilder result)
        {
            var currValue = curr.GetType().GetProperty(propName).GetValue(curr, null);
            var otherValue = other.GetType().GetProperty(propName).GetValue(other, null);
            if ((currValue?.ToString() ?? "null") != (otherValue?.ToString() ?? "null"))
            {
                result.AppendLine($"{propName}: {currValue} -> {otherValue}");
            }
        }

    }
}

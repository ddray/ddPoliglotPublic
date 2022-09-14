using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class WordPhrase
    {
        [Key]
        public int WordPhraseID { get; set; }

        public int LanguageID { get; set; }

        [ForeignKey("LanguageID")]
        public Language Language { get; set; }

        [MaxLength(350)]
        public string Text { get; set; }
        
        public Int64 HashCode { get; set; }

        [MaxLength(100)]
        public string TextSpeechFileName { get; set; }

        public int SpeachDuration { get; set; }

        public Int64 HashCodeSpeed1 { get; set; }

        [MaxLength(100)]
        public string TextSpeechFileNameSpeed1 { get; set; }

        public int SpeachDurationSpeed1 { get; set; }

        public Int64 HashCodeSpeed2 { get; set; }

        [MaxLength(100)]
        public string TextSpeechFileNameSpeed2 { get; set; }

        public int SpeachDurationSpeed2 { get; set; }

        public int SourceType { get; set; } // 0 - dict, 1 - api, 2 - by hand

        [MaxLength(3000)]
        public string WordsUsed { get; set; }

        public List<WordPhraseWord> WordPhraseWords { get; set; } = new List<WordPhraseWord>();

        [NotMapped]
        public WordPhraseTranslation WordPhraseTranslation { get; set; }

        [NotMapped]
        public int PhraseOrderInCurrentWord { get; set; }

        [NotMapped]
        public int CurrentWordID { get; set; }

        [NotMapped]
        public WordPhraseWord WordPhraseWordInCurrentWord { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public string Compare(WordPhrase other)
        {
            string[] propNames = new string[] {
                "LanguageID",
                "Text",
                "HashCode",
                "TextSpeechFileName",
                "SpeachDuration",
                "HashCodeSpeed1",
                "TextSpeechFileNameSpeed1",
                "SpeachDurationSpeed1",
                "HashCodeSpeed2",
                "TextSpeechFileNameSpeed2",
                "SpeachDurationSpeed2",
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
            try
            {
                var currValue = curr.GetType().GetProperty(propName).GetValue(curr, null);
                var otherValue = other.GetType().GetProperty(propName).GetValue(other, null);
                if ((currValue?.ToString() ?? "null") != (otherValue?.ToString() ?? "null"))
                {
                    result.AppendLine($"{propName}: {currValue} -> {otherValue}");
                }
            }
            catch (Exception e)
            {
                throw new Exception($"!!!!!error: {((WordPhrase)curr)?.Text}/{((WordPhrase)other)?.Text}, {propName} : {e.ToString()}, {e.StackTrace.ToString()}");
            }
        }
    }
}

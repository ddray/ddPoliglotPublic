using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ddPoliglotV6.Data.Models
{
    public class Word
    {
        [Key]
        public int WordID { get; set; }
        
        public int LanguageID { get; set; }
        
        [ForeignKey("LanguageID")]
        public Language Language { get; set; }
        
        [MaxLength(250)]
        public string Text { get; set; }
                
        public int Rate { get; set; }

        [MaxLength(50)]
        public string Pronunciation { get; set; }

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

        public int RateTmp { get; set; }

        public int RateTmp2 { get; set; }
        public int RateTmp3 { get; set; }
        public int OxfordLevel { get; set; } // a1, a2, b1, b2 ...

        [MaxLength(250)]
        public string TextClear { get; set; }
        public bool Deleted { get; set; }

        public List<WordPhraseWord> WordPhraseWords { get; set; } = new List<WordPhraseWord>();
        
        [NotMapped]
        public WordTranslation WordTranslation { get; set; }

        [NotMapped]
        public WordUser WordUser { get; set; }

        [NotMapped]
        public List<WordPhrase> WordPhraseSelected { get; set; }

        [NotMapped]
        public List<Word> NeedUseInPhrases { get; set; } // used in schema pharases

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public string Compare(Word other)
        {
            string[] propNames = new string[] {
                "LanguageID",
                "Text",
                "Rate",
                "Pronunciation",
                "HashCode",
                "TextSpeechFileName",
                "SpeachDuration",
                "HashCodeSpeed1",
                "TextSpeechFileNameSpeed1",
                "SpeachDurationSpeed1",
                "HashCodeSpeed2",
                "TextSpeechFileNameSpeed2",
                "SpeachDurationSpeed2",
                "RateTmp",
                "RateTmp2",
                "RateTmp3",
                "OxfordLevel",
                "TextClear",
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
                throw new Exception($"!!!!!error: {((Word)curr).TextClear}, {propName} : {e.ToString()}, {e.StackTrace.ToString()}");
            }
        }
    }
}

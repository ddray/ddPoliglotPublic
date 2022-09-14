using ddPoliglotV6.Data.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ddPoliglotV6.BL.Models
{
    public class WordsForPhraseStateData
    {
        [JsonProperty("waiting")]
        public List<Word> Waiting { get; set; }

        [JsonProperty("excludeFromWaiting")]
        public List<int> ExcludeFromWaiting { get; set; }

        [JsonProperty("currentWord")]
        public Word CurrentWord { get; set; }

        [JsonProperty("phraseTexts")]
        public string PhraseTexts { get; set; }

        [JsonProperty("goToRate")]
        public int? GoToRate { get; set; }
    }
}

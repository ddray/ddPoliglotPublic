using System.Collections.Generic;

namespace ddPoliglotV6.BL.Models
{
    public class TextToSpeechArticlePhraseArg
    {
        public int? ArticleID { get; set; }
        public int? ArticlePhraseID { get; set; }
        public List<int> SelectedArticlePhraseIDs { get; set; }
        public string SessionID { get; set; }
        public string BaseRootPath { get; set; }
    }
}

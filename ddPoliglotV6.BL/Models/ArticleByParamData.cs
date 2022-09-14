using ddPoliglotV6.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ddPoliglotV6.BL.Models
{
    public class ArticleByParamData
    {
        public List<WordSelected> SelectedWords { get; set; }
        public string DialogText { get; set; }
        public string PhrasesText { get; set; }
        public string PhrasesTranslationText { get; set; }
        public List<MixParam> MixParamsList { get; set; }
        public string BaseName { get; set; }
        public string FirstDictorPhrases { get; set; }
        public string BeforeFinishDictorPhrases { get; set; }
        public string FinishDictorPhrases { get; set; }
        public List<Word> WordsToRepeat { get; set; }
        public List<WordPhrase> WordPhrasesToRepeat { get; set; }
        public int? MaxWordsToRepeatForGeneration { get; set; }
    }

    public class WordSelected
    {
        public Word Word { get; set; }
        public List<WordPhrase> PhraseWords { get; set; }
        public List<WordPhrase> PhraseWordsSelected { get; set; }
    }
}

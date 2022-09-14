using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ddPoliglotV6.Data.Models
{
    public class MixParam
    {
        public int MixParamID { get; set; }
        [MaxLength(50)]
        public string ArticlePhraseKeyGuid { get; set; }
        public ICollection<MixItem> MixItems { get; set; }
        public Int64 TextHashCode { get; set; }
        public Int64 TrTextHashCode { get; set; }
        public bool TrFirst { get; set; }
        public bool Active { get; set; }
        public bool M0 { get; set; }
        public bool M0_repeat { get; set; }
        public bool M01 { get; set; }
        public bool M01_repeat { get; set; }
        public bool M012 { get; set; }
        public bool M012_repeat { get; set; }
        public bool M0123 { get; set; }
        public bool M0123_repeat { get; set; }
        public bool TrActive { get; set; }
        public bool TrM0 { get; set; }
        public bool TrM0_repeat { get; set; }
        public bool TrM01 { get; set; }
        public bool TrM01_repeat { get; set; }
        public bool TrM012 { get; set; }
        public bool TrM012_repeat { get; set; }
        public bool TrM0123 { get; set; }
        public bool TrM0123_repeat { get; set; }
        [MaxLength(300)]
        public string Offer1 { get; set; }
        [MaxLength(300)]
        public string Offer2 { get; set; }
        [MaxLength(300)]
        public string Offer3 { get; set; }
        [MaxLength(300)]
        public string Offer4 { get; set; }
        [MaxLength(300)]
        public string Offer5 { get; set; }
        [MaxLength(300)]
        public string TrOffer1 { get; set; }
        [MaxLength(300)]
        public string TrOffer2 { get; set; }
        [MaxLength(300)]
        public string TrOffer3 { get; set; }
        [MaxLength(300)]
        public string TrOffer4 { get; set; }
        [MaxLength(300)]
        public string TrOffer5 { get; set; }
        public int PhrasesMixType { get; set; } // mix-1 wave (for texts), mix-2 rnd (for words)
        public int Repeat { get; set; } // use items in rnd repeat
        public int TrRepeat { get; set; } // use items in rnd repeat
        public int RepeatOrder { get; set; } // repeat in order for mix-2
        public int TrRepeatOrder { get; set; } // repeat in order for mix-2
        public bool addSlowInRepeatOrder { get; set; } // speack slow in order for mix-2
        public bool addSlow2InRepeatOrder { get; set; } // speack slow in order for mix-2
        public bool TrAddSlowInRepeatOrder { get; set; } // speack slow in order for mix-2
        public bool TrAddSlow2InRepeatOrder { get; set; } // speack slow in order for mix-2
        public int RepeatBaseWord { get; set; } // add rnd repeat of base words only for mix-2
        public int TrRepeatBaseWord { get; set; } // add rnd repeat of base words only for mix-2
        public string firstDictorPhrases { get; set; }
        public string firstBeforeDialogDictorPhrases { get; set; }

        public string beforeByOrderMixDictorPhrases { get; set; }
        public string insideByOrderMixDictorPhrases { get; set; }

        public string beforeBaseWordsDirMixDictorPhrases { get; set; }
        public string insideBaseWordsDirMixDictorPhrases { get; set; }

        public string beforeBaseWordsRevMixDictorPhrases { get; set; }
        public string insideBaseWordsRevMixDictorPhrases { get; set; }

        public string beforeAllDirMixDictorPhrases { get; set; }
        public string insideAllDirMixDictorPhrases { get; set; }

        public string beforeAllRevMixDictorPhrases { get; set; }
        public string insideAllRevMixDictorPhrases { get; set; }

        public string beforeFinishDictorPhrases { get; set; }
        public string finishDictorPhrases { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> firstDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> firstBeforeDialogDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeByOrderMixDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> insideByOrderMixDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeBaseWordsDirMixDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> insideBaseWordsDirMixDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeBaseWordsRevMixDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> insideBaseWordsRevMixDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeAllDirMixDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> insideAllDirMixDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeAllRevMixDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> insideAllRevMixDictorPhrasesWithAudio { get; set; }

        [NotMapped]
        public List<List<List<ArticlePhrase>>> beforeFinishDictorPhrasesWithAudio { get; set; }
        [NotMapped]
        public List<List<List<ArticlePhrase>>> finishDictorPhrasesWithAudio { get; set; }





        [NotMapped]
        public List<List<List<APhWr>>> firstDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> firstBeforeDialogDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeByOrderMixDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> insideByOrderMixDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeBaseWordsDirMixDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> insideBaseWordsDirMixDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeBaseWordsRevMixDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> insideBaseWordsRevMixDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeAllDirMixDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> insideAllDirMixDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeAllRevMixDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> insideAllRevMixDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<List<List<APhWr>>> beforeFinishDictorPhrasesWithAudioW { get; set; }
        [NotMapped]
        public List<List<List<APhWr>>> finishDictorPhrasesWithAudioW { get; set; }

        [NotMapped]
        public List<APhWr> tmpList { get; set; } = new List<APhWr>();

        public void MoveToWrappers()
        {
            firstDictorPhrasesWithAudioW = ToWr(firstDictorPhrasesWithAudio);
            firstBeforeDialogDictorPhrasesWithAudioW = ToWr(firstBeforeDialogDictorPhrasesWithAudio);
            beforeByOrderMixDictorPhrasesWithAudioW = ToWr(beforeByOrderMixDictorPhrasesWithAudio);
            insideByOrderMixDictorPhrasesWithAudioW = ToWr(insideByOrderMixDictorPhrasesWithAudio);
            beforeBaseWordsDirMixDictorPhrasesWithAudioW = ToWr(beforeBaseWordsDirMixDictorPhrasesWithAudio);
            insideBaseWordsDirMixDictorPhrasesWithAudioW = ToWr(insideBaseWordsDirMixDictorPhrasesWithAudio);
            beforeBaseWordsRevMixDictorPhrasesWithAudioW = ToWr(beforeBaseWordsRevMixDictorPhrasesWithAudio);
            insideBaseWordsRevMixDictorPhrasesWithAudioW = ToWr(insideBaseWordsRevMixDictorPhrasesWithAudio);
            beforeAllDirMixDictorPhrasesWithAudioW = ToWr(beforeAllDirMixDictorPhrasesWithAudio);
            insideAllDirMixDictorPhrasesWithAudioW = ToWr(insideAllDirMixDictorPhrasesWithAudio);
            beforeAllRevMixDictorPhrasesWithAudioW = ToWr(beforeAllRevMixDictorPhrasesWithAudio);
            insideAllRevMixDictorPhrasesWithAudioW = ToWr(insideAllRevMixDictorPhrasesWithAudio);
            beforeFinishDictorPhrasesWithAudioW = ToWr(beforeFinishDictorPhrasesWithAudio);
            finishDictorPhrasesWithAudioW = ToWr(finishDictorPhrasesWithAudio);
        }

        public List<List<List<APhWr>>> ToWr(List<List<List<ArticlePhrase>>> source) 
        {
            List<List<List<APhWr>>> res = new List<List<List<APhWr>>>();
            foreach (var l1 in source)
            {
                var r1 = new List<List<APhWr>>();
                res.Add(r1);
                foreach (var l2 in l1)
                {
                    var r2 = new List<APhWr>();
                    r1.Add(r2);
                    foreach (var l3 in l2)
                    {
                        var aw = l3.ToWrapper();
                        r2.Add(aw);
                        tmpList.Add(aw);
                    }
                }
            }

            return res;
        }
    }
}

import { PhrasesMixType } from '../enums/Enums';
import { IArticlePhrase } from './IArticlePhrase';

export interface IMixItem {
  mixItemID: number;
  mixParamID: number;
  keyGuid: string;
  source: string;
  inDict: string;
  inContext: string;
  endPhrase: boolean;
  pretext: boolean;
  childrenType: string;
  orderNum: number;
  baseWord: boolean;
}

export interface IMixParams {
  mixParamID: number;
  name?: string;
  articlePhraseKeyGuid: string; // ref to articlePhrase
  textHashCode: number;
  trTextHashCode: number;
  mixItems: Array<IMixItem>;
  trFirst: boolean; // which mix first
  active: boolean; // if we need this type of mix
  m0: boolean;
  m0_repeat: boolean;
  m01: boolean;
  m01_repeat: boolean;
  m012: boolean;
  m012_repeat: boolean;
  m0123: boolean;
  m0123_repeat: boolean;
  trActive: boolean; // if we need this type of mix
  trM0: boolean;
  trM0_repeat: boolean;
  trM01: boolean;
  trM01_repeat: boolean;
  trM012: boolean;
  trM012_repeat: boolean;
  trM0123: boolean;
  trM0123_repeat: boolean;
  offer1: string;
  offer2: string;
  offer3: string;
  offer4: string;
  offer5: string;
  trOffer1: string;
  trOffer2: string;
  trOffer3: string;
  trOffer4: string;
  trOffer5: string;
  phrasesMixType: PhrasesMixType | number; // mix - 1 wave(for texts); mix - 2 rnd(for words)
  repeat: number; // for random mode; how many repeat
  trRepeat: number; // for random mode; how many repeat
  repeatOrder: number; // for order mode; how many repeat
  trRepeatOrder: number; // for order mode; how many repeat
  addSlowInRepeatOrder: boolean; // speack slow in order for mix-2
  addSlow2InRepeatOrder: boolean; // speack slow in order for mix-2
  trAddSlowInRepeatOrder: boolean; // speack slow in order for mix-2
  trAddSlow2InRepeatOrder: boolean; // speack slow in order for mix-2
  repeatBaseWord: number; // add rnd repeat of base words only for mix-2
  trRepeatBaseWord: number;  // add rnd repeat of base words only for mix-2

  firstDictorPhrases: string;
  firstDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  firstBeforeDialogDictorPhrases: string;
  firstBeforeDialogDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeByOrderMixDictorPhrases: string;
  beforeByOrderMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  insideByOrderMixDictorPhrases: string;
  insideByOrderMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeBaseWordsDirMixDictorPhrases: string;
  beforeBaseWordsDirMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  insideBaseWordsDirMixDictorPhrases: string;
  insideBaseWordsDirMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeBaseWordsRevMixDictorPhrases: string;
  beforeBaseWordsRevMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  insideBaseWordsRevMixDictorPhrases: string;
  insideBaseWordsRevMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeAllDirMixDictorPhrases: string;
  beforeAllDirMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  insideAllDirMixDictorPhrases: string;
  insideAllDirMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeAllRevMixDictorPhrases: string;
  beforeAllRevMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  insideAllRevMixDictorPhrases: string;
  insideAllRevMixDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  beforeFinishDictorPhrases: string;
  beforeFinishDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;
  finishDictorPhrases: string;
  finishDictorPhrasesWithAudio: Array<Array<IArticlePhrase>>;

  lastGeneratedArticleId: number;
}

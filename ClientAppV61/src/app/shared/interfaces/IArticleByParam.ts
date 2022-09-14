import { IWordSelected } from './IWordSelected';
import { IMixParams } from './IMixItem';
import { IWord } from './IWord';
import { IWordPhrase } from './IWordPhrase';

export interface IArticleByParam {
  articleByParamID: number;
  userID: string;
  nativeLanguageID: number;
  learnLanguageID: number;
  type: number; // 0 - article by vocabulary, 1 - article by dialog
  name: string;
  dataJson: string;
  isTemplate: boolean;
  isShared: boolean;
  articleByParamData?: IArticleByParamData;
}

export interface IArticleByParamData {
  selectedWords?: Array<IWordSelected>;
  dialogText?: string;
  phrasesText?: string;
  phrasesTranslationText?: string;
  mixParamsList: Array<IMixParams>;
  baseName: string;
  firstDictorPhrases: string;
  beforeFinishDictorPhrases: string;
  finishDictorPhrases: string;
  wordsToRepeat: Array<IWord>;
  maxWordsToRepeatForGeneration: number;
  wordPhrasesToRepeat: Array<IWordPhrase>;
}

export interface IArticleByParamAndDataWithAudio {
data: IArticleByParam;
articleByParamDataWithAudio: IArticleByParamData;
}




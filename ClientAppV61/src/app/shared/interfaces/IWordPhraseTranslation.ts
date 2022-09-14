import { IWordPhrase } from './IWordPhrase';
import { IPlailable } from './IPlailable';

export interface IWordPhraseTranslation  extends IPlailable {
  wordPhraseTranslationID: number;
  wordPhraseID: number;
  languageID: number;
  text: string;
  hashCode: number;
  textSpeechFileName: string;
  speachDuration: number;
  wordPhrase?: IWordPhrase;
}



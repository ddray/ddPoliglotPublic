import { IPlailable } from './IPlailable';

export interface IWordTranslation extends IPlailable {
  wordTranslationID: number;
  wordID: number;
  languageID: number;
  text: string;
  hashCode: number;
  textSpeechFileName: string;
  speachDuration: number;
}


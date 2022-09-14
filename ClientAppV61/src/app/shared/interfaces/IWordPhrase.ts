import { IWordPhraseTranslation } from './IWordPhraseTranslation';
import { IWordUsed } from './IWordUsed';
import { IPlailable } from './IPlailable';

export interface IWordPhrase extends IPlailable {
  wordPhraseID: number;
  languageID: number;
  text: string;
  hashCode: number;
  textSpeechFileName: string;
  speachDuration: number;
  hashCodeSpeed1: number;
  textSpeechFileNameSpeed1: string;
  speachDurationSpeed1: number;
  hashCodeSpeed2: number;
  textSpeechFileNameSpeed2: string;
  speachDurationSpeed2: number;
  selected: boolean;
  wordsUsed: string;
  wordPhraseTranslation?: IWordPhraseTranslation;
  wordsUsedList?: Array<IWordUsed>;
  textHtml?: string;
  phraseOrderInCurrentWord?: number;
  currentWordID?: number;
}

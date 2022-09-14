import { IWordPhrase } from './IWordPhrase';
import { IWordTranslation } from './IWordTranslation';
import { IWordUser } from './IWordUser';
import { IPlailable } from './IPlailable';

export interface IWord extends IPlailable {
  wordID: number;
  languageID: number;
  text: string;
  rate: number;
  pronunciation: string;
  hashCode: number;
  textSpeechFileName: string;
  speachDuration: number;
  hashCodeSpeed1: number;
  textSpeechFileNameSpeed1: string;
  speachDurationSpeed1: number;
  hashCodeSpeed2: number;
  textSpeechFileNameSpeed2: string;
  speachDurationSpeed2: number;
  phraseWords: Array<IWordPhrase>;
  selected?: boolean;
  wordTranslation?: IWordTranslation;
  wordUser?: IWordUser;
  wordPhraseSelected?: Array<IWordPhrase>;
  wordPhraseSelectedIsRedyForLesson?: boolean;
  repeatWordsToIncludeInPhrases?: Array<IWord>;
  repeatPhrases?: Array<IWordPhrase>;
}

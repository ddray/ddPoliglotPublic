import { IWord } from './IWord';

export interface IWordsForPhraseState {
  waiting: Array<IWord>;
  currentWord: IWord;
  excludeFromWaiting: Array<number>;
  phraseTexts: string;
  goToRate?: number;
}






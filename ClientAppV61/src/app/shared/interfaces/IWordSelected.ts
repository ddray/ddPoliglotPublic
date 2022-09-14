import { IWordPhrase } from './IWordPhrase';
import { IWord } from './IWord';

export interface IWordSelected {
  word: IWord;
  phraseWords: Array<IWordPhrase>;
  phraseWordsSelected: Array<IWordPhrase>;
}

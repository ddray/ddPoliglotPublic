import { IWord } from './IWord';

export interface IWordUser {
  wordUserID?: number;
  wordID: number;
  word?: IWord;
  userID?: string;
  languageID?: number;
  grade: number;
}



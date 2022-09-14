import { IArticlePhrase } from './IArticlePhrase';
import { IArticleActor } from './IArticleActor';

export interface IArticle {
  articleID: number;
  userID: string;
  name: string;
  language: string;
  languageTranslation: string;
  textHashCode: string;
  textSpeechFileName: string;
  videoFileName: string;
  articlePhrases: IArticlePhrase[];
  articleActors: IArticleActor[];
}

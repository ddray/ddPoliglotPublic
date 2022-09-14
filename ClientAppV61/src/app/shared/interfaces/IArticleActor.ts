import { IArticle } from './IArticle';

export interface IArticleActor {
  articleActorID: number,
  keyGuid: string,
  name: string,
  role: number,
  defaultInRole: boolean,
  language: string,
  voiceName: string,
  voiceSpeed: number,
  voicePitch: number,
  articleID: number,
  article: IArticle,
}


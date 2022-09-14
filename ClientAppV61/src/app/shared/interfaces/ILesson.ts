import { IArticleByParam } from './IArticleByParam';

export interface ILesson {
  lessonID: number;
  parentID: number;
  languageID: number;
  nativeLanguageID: number; // native user language
  name: string;
  pageName: string;
  description: string;
  content: string;
  order: number;
  video1: string;
  audio1: string;
  description1: string;
  video2: string;
  audio2: string;
  description2: string;
  video3: string;
  audio3: string;
  description3: string;
  video4: string;
  audio4: string;
  description4: string;
  video5: string;
  audio5: string;
  image1: string;
  image2: string;
  image3: string;
  image4: string;
  image5: string;
  description5: string;
  articleByParamID: number;
  articleByParam: IArticleByParam;
  pageMetaTitle: string;
  pageMetaDescription: string;
}

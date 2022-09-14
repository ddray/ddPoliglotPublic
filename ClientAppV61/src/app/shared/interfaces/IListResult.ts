import { IArticle } from './IArticle';

export interface IListResult {
  count: number,
  data: Array<IArticle | any>
}


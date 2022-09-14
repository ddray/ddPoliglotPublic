export interface IListArg {
  sort?: string;
  order?: string;
  page?: number;
  pagesize?: number;
}

export interface ISearchListArg extends IListArg {
  parentID?: number;
  languageID?: number;
  searchText?: string;
  rateFrom?: number;
  rateTo?: number;
  gradeFrom?: number;
  gradeTo?: number;
  recomended?: boolean;
  str1?: string;
  str2?: string;
  int1?: number;
  int2?: number;
}

export interface IArtParamsGenerationArg
{
    articleByParamID?: number;
    baseName?: string;
    startWordRate?: number;
    startLessonNum?: number;
    endLessonNum?: number;
    wordsByLesson?: number;
    maxWordsForRepetition?: number;
    str1?: string;
    str2?: string;
    int1?: number;
    int2?: number;
}

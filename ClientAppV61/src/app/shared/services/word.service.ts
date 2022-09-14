import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IListArg, ISearchListArg } from '../interfaces/IListArg';
import { IListResult } from '../interfaces/IListResult';
import { environment } from '../../../environments/environment';
import { IWord } from '../interfaces/IWord';
import { IWordSelected } from '../interfaces/IWordSelected';
import { IWordTranslation } from '../interfaces/IWordTranslation';
import { IWordPhraseTranslation } from '../interfaces/IWordPhraseTranslation';
import { IWordUser } from '../interfaces/IWordUser';
import { IWordPhrase } from '../interfaces/IWordPhrase';

@Injectable({
  providedIn: 'root'
})
export class WordService {
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  getAll(data: IListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Word/GetAll', { params: { ...data } as any });
  }

  getById(id: string): Observable<IWord> {
    return this.http.get<IWord>(this.baseUrl + '/Word/GetById', { params: { id } as any });
  }

  Save(word: IWord): Observable<IWord> {
    return this.http.post<IWord>(this.baseUrl + '/Word/Save', { ...word });
  }

  Delete(word: IWord) {
    return this.http.post(this.baseUrl + '/Word/Delete', { ...word });
  }

  makeSpeeches(word: IWord) {
    return this.http.post(this.baseUrl + '/Word/MakeSpeeches', { wordID: word.wordID } as IWord);
  }

  getFiltered(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Word/GetFiltered', { params: { ...data } as any });
  }

  getRecomended(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Word/GetRecomended', { params: { ...data } as any });
  }

  getWordPhrases(data: ISearchListArg): Observable<IListResult> {
    return this.http.post<IListResult>(this.baseUrl + '/Word/GetWordPhrases', { ...data });
  }

  updateWordTranslation(data: IWordTranslation) {
    return this.http.post<IWordTranslation>(this.baseUrl + '/Word/UpdateWordTranslation', { ...data });
  }

  updateWordPhraseTranslation(data: IWordPhraseTranslation) {
    return this.http.post<IWordPhraseTranslation>(this.baseUrl + '/Word/UpdateWordPhraseTranslation', { ...data });
  }

  updateWordUser(data: IWordUser) {
    return this.http.post<IWordUser>(this.baseUrl + '/WordUser/UpdateWordUser', { ...data });
  }

  SaveWordPhrase(data: IWordPhrase) {
    return this.http.post<IWordPhrase>(this.baseUrl + '/Word/SaveWordPhrase', { ...data });
  }

  addListFromTextWithPhrases(data: ISearchListArg) {
    return this.http.post(this.baseUrl + '/Word/AddListFromTextWithPhrases', { ...data });
  }

  getListForRepetition(data: ISearchListArg): Observable<IWord[]> {
    return this.http.post<IWord[]>(this.baseUrl + '/Word/GetListForRepetition', { ...data });
  }
}

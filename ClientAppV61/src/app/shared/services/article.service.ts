import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IArtParamsGenerationArg, IListArg } from '../interfaces/IListArg';
import { IListResult } from '../interfaces/IListResult';
import { IArticle } from '../interfaces/IArticle';
import { environment } from '../../../environments/environment';
import { IMixParams } from '../interfaces/IMixItem';
import { IWorkJobsStateEntity } from '../interfaces/IWorkJobsStateEntity';

@Injectable({
  providedIn: 'root'
})
export class ArticleService {
  
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  getAll(data: IListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Article/GetAll', { params: { ...data } as any });
  }

  getTick() {
    return this.http.get(this.baseUrl + '/Article/GetTick');
  }

  getById(id: string): Observable<IArticle> {
    return this.http.get<IArticle>(this.baseUrl + '/Article/GetById', { params: { id } as any });
  }

  Save(article: IArticle): Observable<IArticle> {
    return this.http.post<IArticle>(this.baseUrl + '/Article/Save', { ...article });
  }

  Delete(id) {
    return this.http.post<IArticle>(this.baseUrl + '/Article/Delete',
    {
      articleID: id,
      language: '',
      articlePhrases: [], languageTranslation: '', name: '', textHashCode: '', textSpeechFileName: ''
    } as IArticle
     );
  }

  textToSpeachArticlePhrase(articlePhraseID) {
    return this.http.post(this.baseUrl + '/Article/text_to_speach_article_phrase', { articlePhraseID });
  }

  generateVideoAndAudio(args: IArtParamsGenerationArg) {
    return this.http.post(this.baseUrl + '/Article/GenerateVideoAndAudio', { ...args });
  }

  textToSpeachArticle(articleID, selectedArticlePhraseIDs, sessionID) {
    return this.http.post(this.baseUrl + '/Article/textToSpeachArticle',
     { articleID, SelectedArticlePhraseIDs: selectedArticlePhraseIDs, sessionID });
  }

  textToSpeachArticleRetrialState(sessionID: string) {
    return this.http.post<IWorkJobsStateEntity>(this.baseUrl + '/Article/textToSpeachArticleRetrialState', { sessionID });
  }

  textToVideoArticle(articleID: number, selectedArticlePhraseIDs: number[], sessionID) {
    return this.http.post(this.baseUrl + '/Article/text_to_video_article',
    { articleID, SelectedArticlePhraseIDs: selectedArticlePhraseIDs, sessionID });
  }

  textToVideoArticleRetrialState(sessionID: string) {
    return this.http.post<IWorkJobsStateEntity>(this.baseUrl + '/Article/textToVideoArticleRetrialState', { sessionID });
  }
}

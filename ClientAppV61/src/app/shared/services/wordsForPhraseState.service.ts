import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { IWordsForPhraseState } from '../interfaces/IWordsForPhraseState';

@Injectable({
  providedIn: 'root'
})
export class WordsForPhraseStateService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api/WordsForPhraseState';
  }

  GetNext(): Observable<IWordsForPhraseState> {
    return this.http.get<IWordsForPhraseState>(this.baseUrl + '/GetNext');
  }

  Save(wordsForPhraseState: IWordsForPhraseState): Observable<any> {
    return this.http.post(this.baseUrl + '/Save', { ...wordsForPhraseState });
  }

  GoToRate(wordsForPhraseState: IWordsForPhraseState): Observable<any> {
    return this.http.post(this.baseUrl + '/GoToRate', { ...wordsForPhraseState });
  }
}

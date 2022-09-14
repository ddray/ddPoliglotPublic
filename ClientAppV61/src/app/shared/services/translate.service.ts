import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ITranslateArg } from '../interfaces/ITranslateArg';
import { ITranslateResult } from '../interfaces/ITranslateResult';
import { Observable } from 'rxjs';
import { ITranslateArrayArg } from '../interfaces/ITranslateArrayArg';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class TranslateService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  translate(data: ITranslateArg): Observable<ITranslateResult> {
    return this.http.post<ITranslateResult>(this.baseUrl + '/Translate/Translate', { ...data });
  }

  translateArray(data: ITranslateArrayArg): Observable<Array<string>> {
    return this.http.post<Array<string>>(this.baseUrl + '/Translate/TranslateArray', { ...data });
  }
}

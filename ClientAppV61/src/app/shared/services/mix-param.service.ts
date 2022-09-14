import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

import { IMixParams } from '../interfaces/IMixItem';
import { IItemArgs } from '../interfaces/IItemArgs';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class MixParamService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  saveMixParam(mixParams: IMixParams) {
    return this.http.post(this.baseUrl + '/MixParam/SaveMixParam', { ...mixParams });
  }

  getByArticlePhraseKeyGuid(articlePhraseKeyGuid: string) {
    return this.http.post<IMixParams>(this.baseUrl + '/MixParam/GetByArticlePhraseKeyGuid', { str1: articlePhraseKeyGuid } as IItemArgs);
  }
}

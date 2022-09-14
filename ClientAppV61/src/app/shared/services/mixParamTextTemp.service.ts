import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { IListResult } from '../interfaces/IListResult';
import { environment } from '../../../environments/environment';
import { IMixParamTextTemp } from '../interfaces/IMixParamTextTemp';
import { ISearchListArg } from '../interfaces/IListArg';

@Injectable({
  providedIn: 'root'
})
export class MixParamTextTempService {
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  baseUrl: string;
  private apiControlerUrl = '';

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
    this.apiControlerUrl = `${this.baseUrl}/MixParamTextTemp/`;
  }

  getFiltered(data: ISearchListArg) {
    return this.http.post<IListResult>(this.apiControlerUrl + 'GetFiltered', { ...data });
  }

  Save(data: IMixParamTextTemp) {
    return this.http.post<IMixParamTextTemp>(this.apiControlerUrl + 'Save', { ...data });
  }

  Delete(data: IMixParamTextTemp) {
    return this.http.post<IMixParamTextTemp>(this.apiControlerUrl + 'Delete', { ...data });
  }
}

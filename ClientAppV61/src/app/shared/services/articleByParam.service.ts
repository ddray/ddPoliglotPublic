import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IArtParamsGenerationArg, IListArg, ISearchListArg } from '../interfaces/IListArg';
import { IListResult } from '../interfaces/IListResult';
import { environment } from '../../../environments/environment';
import { IArticleByParam, IArticleByParamAndDataWithAudio } from '../interfaces/IArticleByParam';

@Injectable({
  providedIn: 'root'
})
export class ArticleByParamService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  getByIdWithReadyAudio(id: string) {
    return this.http.get<IArticleByParamAndDataWithAudio>(this.baseUrl + '/ArticleByParam/GetByIdWithReadyAudio', { params: { id } });
  }

  getById(id: string): Observable<IArticleByParam> {
    return this.http.get<IArticleByParam>(this.baseUrl + '/ArticleByParam/GetById', { params: { id } });
  }

  save(item: IArticleByParam): Observable<IArticleByParam> {
    return this.http.post<IArticleByParam>(this.baseUrl + '/ArticleByParam/Save', { ...item });
  }

  delete(item: IArticleByParam) {
    return this.http.post(this.baseUrl + '/ArticleByParam/Delete', { ...item });
  }

  copy(item: IArticleByParam) {
    return this.http.post(this.baseUrl + '/ArticleByParam/Copy', { ...item });
  }

  generate(item: IArtParamsGenerationArg) {
    return this.http.post(this.baseUrl + '/ArticleByParam/Generate', { ...item });
  }
  
  getFiltered(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/ArticleByParam/GetFiltered', { params: { ...data } as any });
  }
}

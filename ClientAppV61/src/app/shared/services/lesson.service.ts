import { Injectable, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IArtParamsGenerationArg, IListArg, ISearchListArg } from '../interfaces/IListArg';
import { IListResult } from '../interfaces/IListResult';
import { environment } from '../../../environments/environment';
import { ILesson } from '../interfaces/ILesson';

@Injectable({
  providedIn: 'root'
})
export class LessonService {
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  getAll(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Lesson/GetAll', { params: { ...data } as any });
  }

  getById(id: string): Observable<ILesson> {
    return this.http.get<ILesson>(this.baseUrl + '/Lesson/GetById', { params: { id }  as any });
  }

  Save(word: ILesson): Observable<ILesson> {
    return this.http.post<ILesson>(this.baseUrl + '/Lesson/Save', { ...word });
  }

  Delete(word: ILesson) {
    return this.http.post(this.baseUrl + '/Lesson/Delete', { ...word });
  }

  generateFromArticles(item: IArtParamsGenerationArg) {
    return this.http.post(this.baseUrl + '/Lesson/GenerateFromArticles', { ...item });
  }

  uploadVideosToYoutube(item: IArtParamsGenerationArg) {
    return this.http.post(this.baseUrl + '/Lesson/uploadVideosToYoutube', { ...item });
  }

  getFiltered(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Lesson/GetFiltered', { params: { ...data } as any });
  }

  getAudioFileList(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/Lesson/GetAudioFileList', { params: { ...data } as any });
  }

  AudioUpload(formData: FormData) {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data');
    headers.append('Accept', 'application/json');
    return this.http.post(this.baseUrl + '/Lesson/AudioUpload', formData, {headers, reportProgress: true, observe: 'events', });
  }

  FileUpload(formData: FormData, data: ISearchListArg) {
    const headers = new HttpHeaders();
    headers.append('Content-Type', 'multipart/form-data');
    headers.append('Accept', 'application/json');
    return this.http
      .post(this.baseUrl + '/Lesson/FileUpload'
        , formData, {headers, reportProgress: true, observe: 'events', params: { ...data } as any});
  }
}

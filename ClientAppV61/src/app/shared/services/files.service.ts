import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, BehaviorSubject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ILanguage } from '../interfaces/ILanguage';
import { IVoiceByLanguage } from '../interfaces/IVoiceByLanguage';
import { IArticleActor } from '../interfaces/IArticleActor';
import { IArticle } from '../interfaces/IArticle';
import { Guid } from '../models/System';
import { ArticleActorRole } from '../enums/Enums';
import { IAppSettings } from '../interfaces/IAppSettings';

@Injectable({
  providedIn: 'root'
})
export class FilesService {
  baseUrl: string;

  constructor(
    private http: HttpClient
  ) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  getPhraseAudio(fileName: string) {
    return this.http.get(this.baseUrl + '/Files/GetPhraseAudio', { params: { fileName } as any });
  }
}

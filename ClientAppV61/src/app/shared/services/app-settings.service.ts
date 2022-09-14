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
export class AppSettingsService {
  baseUrl: string;

  public currentAppSetting = new BehaviorSubject<IAppSettings>(undefined as IAppSettings);

  constructor(
    private http: HttpClient
  ) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  public getAppSettingsFromStore(currUser): IAppSettings {
    let res: IAppSettings;
    const storeName = this.GetStoreNameByUser(currUser);
    const str = localStorage.getItem(storeName);
    if (str) {
      res = JSON.parse(str);
    }

    return res;
  }

  public getCurrent(): IAppSettings {
    return this.currentAppSetting.getValue();
  }

  public refreshCurrent(newValue: IAppSettings): void {
    this.currentAppSetting.next(newValue);
  }

  public updateAppSettingsInStore(appSettings: IAppSettings, currUser) {
    const storeName = this.GetStoreNameByUser(currUser);
    const str = JSON.stringify(appSettings);
    localStorage.setItem(storeName, str);
    this.refreshCurrent(appSettings);
  }

  GetStoreNameByUser(currUser: any): string {
    return `appSetting_${currUser.sub}`;
  }
}

import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ILanguage } from '../interfaces/ILanguage';
import { IVoiceByLanguage } from '../interfaces/IVoiceByLanguage';
import { IArticleActor } from '../interfaces/IArticleActor';
import { IArticle } from '../interfaces/IArticle';
import { Guid } from '../models/System';
import { ArticleActorRole } from '../enums/Enums';

@Injectable({
  providedIn: 'root'
})
export class GlobalService {
  public phrasesAudioUrl: string = environment.phrasesAudioUrl;
  public articlesAudioUrl: string = environment.articlesAudioUrl;
  baseUrl: string;
  private languages: Array<ILanguage>;

  // private languages: Array<ILanguage> = [
  //  {languageID: 1, name: 'English(US, GB)', code: 'en', codeFull: 'en', shortName: 'en-US'},
  //  {languageID: 2, name: 'Russian', code: 'ru', codeFull: 'ru', shortName: 'ru-RU'},
  //  {languageID: 3, name: 'Deutsch(Deutschland)', code: 'de', codeFull: 'de', shortName: 'de-DE'},
  //  {languageID: 4, name: 'Slovenčina(Slovensko)', code: 'sk', codeFull: 'sk', shortName: 'sk-SK'},
  // ];

  private voicesByLanguage: Array<IVoiceByLanguage> = [
    { ln: 'sk', voice: 'sk-SK-Wavenet-A', def: true, defTrans: true, defDict: true,
     defDictTransl: true, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'sk', voice: 'sk-SK-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 2, speed: 1, namePrefix: 'AA' },
    { ln: 'sk', voice: 'sk-SK-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: -2, speed: 1, namePrefix: 'BB' },
    { ln: 'sk', voice: 'sk-SK-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 1.5, speed: 1, namePrefix: 'CC' },
    { ln: 'sk', voice: 'sk-SK-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: -1.5, speed: 1, namePrefix: 'DD' },

    { ln: 'en', voice: 'en-US-Wavenet-A', def: false, defTrans: false, defDict: true,
     defDictTransl: true, sex: 'm', pitch: 0, speed: 1, namePrefix: 'AM1' },
    { ln: 'en', voice: 'en-US-Wavenet-B', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: 'AM2' },
    { ln: 'en', voice: 'en-US-Wavenet-C', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: 'AF1' },
    { ln: 'en', voice: 'en-US-Wavenet-D', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: 'AM3' },
    { ln: 'en', voice: 'en-US-Wavenet-E', def: true, defTrans: true, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: 'AF2' },
    { ln: 'en', voice: 'en-US-Wavenet-F', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: 'AF3' },

    { ln: 'en', voice: 'en-GB-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: 'GF1' },
    { ln: 'en', voice: 'en-GB-Wavenet-B', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: 'GM1' },
    { ln: 'en', voice: 'en-GB-Wavenet-C', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: 'GF2' },
    { ln: 'en', voice: 'en-GB-Wavenet-D', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: 'GM2' },

    { ln: 'ru', voice: 'ru-RU-Wavenet-A', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'ru', voice: 'ru-RU-Wavenet-B', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'ru', voice: 'ru-RU-Wavenet-C', def: true, defTrans: true, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'ru', voice: 'ru-RU-Wavenet-D', def: false, defTrans: false, defDict: true,
     defDictTransl: true, sex: 'm', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'ru', voice: 'ru-RU-Wavenet-E', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },

    { ln: 'de', voice: 'de-DE-Wavenet-A', def: true, defTrans: true, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'de', voice: 'de-DE-Wavenet-B', def: false, defTrans: false, defDict: true,
     defDictTransl: true, sex: 'm', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'de', voice: 'de-DE-Wavenet-C', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'f', pitch: 0, speed: 1, namePrefix: '' },
    { ln: 'de', voice: 'de-DE-Wavenet-D', def: false, defTrans: false, defDict: false,
     defDictTransl: false, sex: 'm', pitch: 0, speed: 1, namePrefix: '' },
  ];


  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api';
  }

  async getLanguages(): Promise<ILanguage[]> {
    if (!this.languages) {
      this.languages = await this.http.get<Array<ILanguage>>(this.baseUrl + '/Language/GetAll').toPromise();
    }

    return this.languages;
  }

  // getLanguageCurrent(): ILanguage {
  //  return this.languages[0];
  // }

  // getLanguageTarget(): ILanguage {
  //  return this.languages[1];
  // }

  GetVoicesByLanguage(language: string): Array<IVoiceByLanguage> {
    return this.voicesByLanguage.filter((v) => v.ln === language);
  }

  getActorsForArticle(article: IArticle): IArticleActor[] {
    const language = article.language;
    const languageTranslation = article.languageTranslation;
    const result: IArticleActor[] = [];

    if (language) {
      // ArticleActorRole.textSpeaker
      const voices = this.GetVoicesByLanguage(language);
      voices.forEach((voice, index) => {
          result.push({
            articleActorID: 0,
            articleID: article.articleID,
            defaultInRole: voice.def,
            keyGuid: Guid.newGuid().ToString(),
            language: article.language,
            name: (voice.namePrefix ? voice.namePrefix + ' ' : '') + `Speaker ${index + 1} (${article.language})`,
            role: ArticleActorRole.textSpeaker,
            voiceName: voice.voice,
            voicePitch: voice.pitch,
            voiceSpeed: voice.speed,
          } as IArticleActor);
        });

      // ArticleActorRole.textDictorSpeaker
      voices.forEach((voice, index) => {
          result.push({
            articleActorID: 0,
            articleID: article.articleID,
            defaultInRole: voice.defDict,
            keyGuid: Guid.newGuid().ToString(),
            language: article.language,
            name: (voice.namePrefix ? voice.namePrefix + ' ' : '') + `Dictor ${index + 1} (${article.language})`,
            role: ArticleActorRole.textDictorSpeaker,
            voiceName: voice.voice,
            voicePitch: voice.pitch,
            voiceSpeed: voice.speed,
          } as IArticleActor);
        });
    }

    if (article.languageTranslation) {
      // ArticleActorRole.textTranslatedSpeaker
      const voicesTransl = this.GetVoicesByLanguage(article.languageTranslation);
      voicesTransl.forEach((voice, index) => {
          result.push({
            articleActorID: 0,
            articleID: article.articleID,
            defaultInRole: voice.defTrans,
            keyGuid: Guid.newGuid().ToString(),
            language: article.languageTranslation,
            name: (voice.namePrefix ? voice.namePrefix + ' ' : '') + `Speaker ${index + 1} (${article.languageTranslation})`,
            role: ArticleActorRole.textTranslatedSpeaker,
            voiceName: voice.voice,
            voicePitch: voice.pitch,
            voiceSpeed: voice.speed,
          } as IArticleActor);
        });

      // ArticleActorRole.textTranslatedDictorSpeaker
      voicesTransl.forEach((voice, index) => {
          result.push({
            articleActorID: 0,
            articleID: article.articleID,
            defaultInRole: voice.defDictTransl,
            keyGuid: Guid.newGuid().ToString(),
            language: article.languageTranslation,
            name: (voice.namePrefix ? voice.namePrefix + ' ' : '') + `Dictor ${index + 1} (${article.languageTranslation})`,
            role: ArticleActorRole.textTranslatedDictorSpeaker,
            voiceName: voice.voice,
            voicePitch: voice.pitch,
            voiceSpeed: voice.speed,
          } as IArticleActor);
        });
    }

    return result;
  }
}

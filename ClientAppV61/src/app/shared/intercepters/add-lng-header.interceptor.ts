import {
  HttpEvent,
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
} from '@angular/common/http';
import { Observable } from 'rxjs';
import { AppSettingsService } from '../services/app-settings.service';
import { Injectable } from '@angular/core';

@Injectable()
export class AddLngHeaderInterceptor implements HttpInterceptor {
  constructor(
    private appSettingsService: AppSettingsService,
  ) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const currAppSetting = this.appSettingsService.getCurrent();
    let value = '0;0';
    if (currAppSetting) {
      value = `${currAppSetting.NativeLanguage.languageID};${currAppSetting.LearnLanguage.languageID}`;
    }

    const clonedRequest = req.clone({ headers: req.headers.set('spa-app-settings', value)});
    return next.handle(clonedRequest);
  }
}

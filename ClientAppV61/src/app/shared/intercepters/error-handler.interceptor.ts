import { Injectable } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

import {
  HttpRequest, HttpHandler,
  HttpEvent, HttpInterceptor, HttpResponse, HttpErrorResponse
} from '@angular/common/http';

import { Observable, throwError, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Injectable()
export class ErrorHandlerInterceptor implements HttpInterceptor {
  constructor(
    private toasterService: ToastrService
  ) { }

  intercept(
    req: HttpRequest<any>,
    next: HttpHandler
  ): Observable<HttpEvent<any>> {

    return next
      .handle(req)
      .pipe(
        tap(evt => {
          if (evt instanceof HttpResponse) {
            if (evt.body && evt.body.success) {
              this.toasterService.success(evt.body.success.message, evt.body.success.title, { positionClass: 'toast-bottom-center' });
            }
          }
        }),
        catchError((err: any) => {
          console.log('err intr:', err);
          if (err instanceof HttpErrorResponse) {
            if (err && err.status) {
              if (err.status === 401) {
                const url = environment.baseRoot + '/Identity/Account/Login?ReturnUrl=' + location.href.split(environment.baseRoot)[1];
                window.open(url, '_self');
              } else if (err.status === 403) {
                const url = environment.baseRoot + '/Identity/Account/AccessDenied';
                window.open(url, '_self');
                return;
              }
            }
            try {
              this.toasterService.error('An error occurred', '', { positionClass: 'toast-bottom-center' });
            } catch (e) {
              this.toasterService.error('An error occurred', '', { positionClass: 'toast-bottom-center' });
            }
          }

          return throwError(err);
        })
      );

  }
}

import { Injectable } from '@angular/core';
import { BehaviorSubject, concat, from, Observable } from 'rxjs';
import { filter, map, mergeMap, take, tap } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { environment } from '../environments/environment';

export interface IUser {
  name?: string;
  id: string;
  roles: string[];
  isLockedOut?: boolean;
}

@Injectable({
  providedIn: 'root'
})
export class AuthorizeService {
  private baseUrl;

  constructor(
    private http: HttpClient
  ) {
    this.baseUrl = environment.baseRoot + '/api' + '/Authorization';
  }

  private userSubject: BehaviorSubject<IUser | null> = new BehaviorSubject(null);

  public isAuthenticated(): boolean {
    return !!this.userSubject.getValue();
  }

  public getCurrentUser() {
    if (!this.userSubject.getValue()) {
      return this.http.get<IUser>(this.baseUrl + '/GetCurrentUser')
        .pipe(
          map(user => {
            this.userSubject.next(user);
            return user;
          })
        );
    } else {
      return this.userSubject.asObservable();
    }
  }

  public isSuperAdmin() {
    const user = this.userSubject.getValue();
    if (user) {
      return user.roles.some(x => x === 'SuperAdmin');
    }

    return false;
  }

  public isAdmin() {
    const user = this.userSubject.getValue();
    if (user) {
      return user.roles.some(x => x === 'Admin' || x === 'SuperAdmin');
    }

    return false;
  }

  public isLessonMaker() {
    const user = this.userSubject.getValue();
    if (user) {
      return user.roles.some(x => x === 'LessonMaker' || x === 'Admin' || x === 'SuperAdmin');
    }

    return false;
  }
}

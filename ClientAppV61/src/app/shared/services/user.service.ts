import { Injectable, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { IListArg, ISearchListArg } from '../interfaces/IListArg';
import { IListResult } from '../interfaces/IListResult';
import { environment } from '../../../environments/environment';
import { IUser } from 'src/api-authorization/authorize.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl: string;

  constructor(private http: HttpClient) {
    this.baseUrl = environment.baseRoot + '/api/User';
  }

  getAll(data: IListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/GetAll', { params: { ...data } as any });
  }

  getById(id: string): Observable<IUser> {
    return this.http.get<IUser>(this.baseUrl + '/GetById', { params: { id } as any });
  }

  Save(data: IUser): Observable<IUser> {
    return this.http.post<IUser>(this.baseUrl + '/Save', { ...data });
  }

  Delete(data: IUser) {
    return this.http.post(this.baseUrl + '/Delete', { ...data });
  }

  getFiltered(data: ISearchListArg): Observable<IListResult> {
    return this.http.get<IListResult>(this.baseUrl + '/GetFiltered', { params: { ...data } as any });
  }

  getAllRoles(): Observable<Array<string>> {
    return this.http.get<Array<string>>(this.baseUrl + '/GetAllRoles');
  }
}

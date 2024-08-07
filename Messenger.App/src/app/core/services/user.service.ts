import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedEntities } from '../classes/pagination.model';
import { UserInfo } from '../classes/user-info.model';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  constructor(private http: HttpClient) {}

  getUsers(
    userName: string,
    page: number,
    pageSize: number
  ): Observable<PagedEntities<UserInfo>> {
    const url = `${environment.apiUrl}api/Users`;
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
      userName: userName,
    };

    return this.http.get<PagedEntities<UserInfo>>(url, { params });
  }
}

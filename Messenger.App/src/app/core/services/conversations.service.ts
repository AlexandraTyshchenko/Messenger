import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { Conversation } from '../classes/conversation.model';
import { environment } from '../../../environments/environment.development';
import { PagedEntities } from '../classes/pagination.model';

@Injectable({
  providedIn: 'root',
})
export class ConversationsService {
  constructor(private http: HttpClient) {}


  GetConversations(page: number, pageSize: number): Observable<PagedEntities<Conversation>> {
    const url = `${environment.apiUrl}/Conversations`;
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString()
    };

    return this.http.get<PagedEntities<Conversation>>(url, { params });
  }
  
}

import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { PagedEntities } from '../classes/pagination.model';
import { Message } from '../classes/message.model';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  constructor(private http: HttpClient) {}

  GetMessages(
    conversationId: string,
    page: number,
    pageSize: number
  ): Observable<PagedEntities<Message>> {
    const url = `${environment.apiUrl}/Conversations/${conversationId}/Messages`;
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };

    return this.http.get<PagedEntities<Message>>(url, { params });
  }
}

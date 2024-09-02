import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedEntities } from '../classes/pagination.model';
import { Message } from '../classes/message.model';
import { MessageDto } from '../classes/message-dto.model';

@Injectable({
  providedIn: 'root',
})
export class MessagesService {
  constructor(private http: HttpClient) {}

  getMessages(
    conversationId: string,
    page: number,
    pageSize: number
  ): Observable<PagedEntities<Message>> {
    const url = `${environment.apiUrl}api/Conversations/${conversationId}/Messages`;
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString(),
    };

    return this.http.get<PagedEntities<Message>>(url, { params });
  }

  sendMessage(
    conversationId: string,
    messageDto: MessageDto
  ): Observable<Message> {
    const url = `${environment.apiUrl}api/Conversations/${conversationId}/Messages`;

    const formData = new FormData();
    formData.append('text', messageDto.text);
    if (messageDto.image) {
      formData.append('image', messageDto.image);
    }

    return this.http.post<Message>(url, formData);
  }
}

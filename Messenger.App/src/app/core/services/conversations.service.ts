import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { Conversation } from '../classes/conversation.model';
import { environment } from '../../../environments/environment.development';
import { PagedEntities } from '../classes/pagination.model';
import { GroupDto } from '../classes/group-dto.model';
import { UserDto } from '../classes/user-dto.model';

@Injectable({
  providedIn: 'root',
})

export class ConversationsService {
  constructor(private http: HttpClient) {}
  private conversationSubject = new Subject<Conversation>();

  public conversation$ = this.conversationSubject.asObservable();

  getConversations(page: number, pageSize: number): Observable<PagedEntities<Conversation>> {
    const url = `${environment.apiUrl}api/Conversations`;
    const params = {
      page: page.toString(),
      pageSize: pageSize.toString()
    };
    return this.http.get<PagedEntities<Conversation>>(url, { params });
  }
  
  createGroupConversation(groupDto: GroupDto) :Observable<Conversation>{
    const url = `${environment.apiUrl}api/Conversations/group`;
    
    return this.http.post<Conversation>(url, groupDto).pipe(
      tap(conversation => {
        this.conversationSubject.next(conversation);
      })
    );}

    createPrivateConversation(groupDto: UserDto) :Observable<Conversation>{
      const url = `${environment.apiUrl}api/Conversations/private`;
      
      return this.http.post<Conversation>(url, groupDto).pipe(
        tap(conversation => {
          this.conversationSubject.next(conversation);
        })
      );}
}  
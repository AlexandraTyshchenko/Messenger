import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ParticipantsInConversationDto } from '../classes/participants-in-conversation.model';
import { Participant } from '../classes/participant.model';

@Injectable({
  providedIn: 'root',
})
export class ParticipantService {
  private participantDeletedSubject = new Subject<Participant>();

  public participantDeleted$ = this.participantDeletedSubject.asObservable();

  constructor(private http: HttpClient) {}

  addParticipant(
    conversationId: string,
    userIds: string[]
  ): Observable<ParticipantsInConversationDto> {
    const url = `${environment.apiUrl}api/conversations/${conversationId}/Participants`;
    return this.http.post<ParticipantsInConversationDto>(url, userIds);
  }

  getAllParticipants(conversationId: string): Observable<Participant[]> {
    const url = `${environment.apiUrl}api/conversations/${conversationId}/Participants`;
    return this.http.get<Participant[]>(url);
  }

  deleteParticipant(conversationId: string, userId: string): Observable<void> {
    const url = `${environment.apiUrl}api/conversations/${conversationId}/participants/${userId}`;
    return this.http.delete<void>(url).pipe(
      tap(() => this.participantDeletedSubject.next({ userInfo: { id: userId } } as Participant))
    );
  }

  leaveConversation(conversationId: string): Observable<void> {
    const url = `${environment.apiUrl}api/conversations/${conversationId}/participants`;
    return this.http.delete<void>(url);
  }
}

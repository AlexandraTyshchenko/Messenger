import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { ParticipantsInConversationDto } from '../classes/participants-in-conversation.model';

@Injectable({
  providedIn: 'root',
})
export class ParticipantService {
  constructor(private http: HttpClient) {}

  addParticipant(
    groupConversationId: string,
    userIds: string[]
  ): Observable<ParticipantsInConversationDto> {
    const url = `${environment.apiUrl}api/conversations/${groupConversationId}/Participants`;
    return this.http.post<ParticipantsInConversationDto>(url, userIds);
  }
}

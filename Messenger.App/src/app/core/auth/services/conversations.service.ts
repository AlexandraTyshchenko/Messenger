import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs"; 
import { Conversation } from "../classes/conversation.model";
import { environment } from "../../../../environments/environment.development";

@Injectable({
  providedIn: 'root',
}) 
export class ConversationsService {
  
  constructor(private http: HttpClient) {}

  GetConversations(): Observable<Conversation[]> {
    return this.http.get<Conversation[]>(`${environment.apiUrl}/Conversations`);
  }
}
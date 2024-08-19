import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, Subject, tap } from 'rxjs';
import { Conversation } from '../classes/conversation.model';
import { environment } from '../../../environments/environment.development';
import { PagedEntities } from '../classes/pagination.model';
import { GroupDto } from '../classes/group-dto.model';
import { UserDto } from '../classes/user-dto.model';
import { Image } from '../classes/image.model';
import { ImageResult } from '../classes/image.result.model';

@Injectable({
  providedIn: 'root',
})

export class ImageService {
  constructor(private http: HttpClient) {}
  sendImage(conversationId: string, image: Image): Observable<ImageResult> {
    const url = `${environment.apiUrl}api/Conversations/${conversationId}/Images`;

    const formData = new FormData();
    formData.append('file', image.file);
    formData.append('text', image.text);

    return this.http.post<ImageResult>(url, formData);
  }
}  
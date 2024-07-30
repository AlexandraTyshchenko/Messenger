import { Observable } from 'rxjs';
import { Message } from '../classes/message.model';

export interface ISignalRService {
  startConnection(): void;
  onjoinGroups(): void;
  onNotificationReceive(): void;
  get message$(): Observable<Message | null>;
}

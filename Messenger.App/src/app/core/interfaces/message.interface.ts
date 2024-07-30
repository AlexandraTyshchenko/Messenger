import { Observable } from "rxjs";
import { Message } from "../classes/message.model";
import { MessageDto } from "../classes/message-dto.model";
import { PagedEntities } from "../classes/pagination.model";

export interface IMessagesService {
    getMessages(conversationId: string, page: number, pageSize: number): Observable<PagedEntities<Message>>;
    sendMessage(conversationId: string, messageDto: MessageDto): Observable<Message>;
    messageSent$: Observable<Message>;
  }
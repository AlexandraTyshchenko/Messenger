import { Component, Input, OnInit } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { AuthService } from '../../core/services/auth.service';
import { MessagesService } from '../../core/services/messages.service';
import { ConversationDataService } from '../../core/services/conversation.data.service';
import { Message } from '../../core/classes/message.model';

@Component({
  selector: 'app-conversation-item',
  templateUrl: './conversation-item.component.html',
  styleUrls: ['./conversation-item.component.css'],
})
export class ConversationItemComponent {

  @Input() conversation!: Conversation;
  lastMessageSenderImgUrl: string | undefined = undefined;

  getTitle(): string {
    return this.conversationDataService.getConversationTitle(this.conversation);
  }

  imgUrl!: string;
  constructor(private conversationDataService: ConversationDataService) {}

  getImage(): string {
    return this.conversationDataService.getConversationImage(this.conversation);
  }
}

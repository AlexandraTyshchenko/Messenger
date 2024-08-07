import { Component, Input, OnInit } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { ConversationDataService } from '../../core/services/conversation.data.service';

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

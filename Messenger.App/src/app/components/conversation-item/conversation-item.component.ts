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
export class ConversationItemComponent implements OnInit {
  @Input() conversation!: Conversation;
  lastMessageSenderImgUrl: string | undefined = undefined;

  getTitle(): string {
    return this.conversationDataService.getConversationTitle(this.conversation);
  }

  imgUrl!: string;
  constructor(
    private authService: AuthService,
    private conversationDataService: ConversationDataService
  ) {}

  ngOnInit(): void {
    this.lastMessageSenderImgUrl = this.conversation.lastMessage?.sender.imgUrl;
  }

  getImage(): string {
    return this.conversationDataService.getConversationImage(this.conversation);
  }
  getSenderDisplayText(): string {
    const userId = this.authService.user()?.nameidentifier;

    if (this.conversation.lastMessage !== null) {
      if (this.conversation.lastMessage.sender?.id === userId) {
        return 'you:';
      } else {
        const firstName = this.conversation.lastMessage?.sender.firstName ?? '';
        const lastName = this.conversation.lastMessage?.sender.lastName ?? '';
        return `${firstName} ${lastName}:`;
      }
    }
    return '';
  }

  getSenderMessageText(): string {
    if (this.conversation.lastMessage !== null)
      return this.conversation.lastMessage?.text;
    return 'messages haven`t been added yet';
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { AuthService } from '../../core/services/auth.service';
import { ConversationDataService } from '../../core/services/conversation.data.service';

@Component({
  selector: 'app-conversation-with-last-message',
  templateUrl: './conversation-with-last-message.component.html',
  styleUrl: './conversation-with-last-message.component.css',
})
export class ConversationWithLastMessageComponent implements OnInit {
  @Input() conversation!: Conversation;
  lastMessageSenderImgUrl: string | undefined = undefined;
  constructor(private authService: AuthService) {}

  ngOnInit(): void {
    this.lastMessageSenderImgUrl = this.conversation.lastMessage?.sender.imgUrl;
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

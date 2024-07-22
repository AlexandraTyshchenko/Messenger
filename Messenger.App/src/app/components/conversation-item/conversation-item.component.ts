import { Component, Input } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-conversation-item',
  templateUrl: './conversation-item.component.html',
  styleUrls: ['./conversation-item.component.css'],
})
export class ConversationItemComponent {
  @Input() conversation!: Conversation;

  constructor(private authService:AuthService) {
    
  }
  getFirstParticipant(conversation: Conversation): string {
    const userName = this.authService.user()?.name;
    if (conversation.participants && conversation.participants.length > 0) {
      const firstNonUserParticipant = conversation.participants.find(x => x.userName !== userName);
      return firstNonUserParticipant ? firstNonUserParticipant.userName : 'Unknown';
    }
    return 'Unknown';
  }

  getSenderDisplayText(conversation: Conversation): string {
    const userId = this.authService.user()?.nameidentifier;
  
    if (conversation.lastMessage !== null) {
      if (conversation.lastMessage.sender?.id === userId) {
        return 'you:';
      } else {
        const firstName = conversation.lastMessage?.sender.firstName ?? '';
        const lastName = conversation.lastMessage?.sender.lastName ?? '';
        return `${firstName} ${lastName}:`;
      }
    }
    return '';
  }

  getSenderMessageText(conversation: Conversation):string{
      if(conversation.lastMessage!==null)
        return  conversation.lastMessage?.text
      return "messages haven`t been added yet"
  }
}

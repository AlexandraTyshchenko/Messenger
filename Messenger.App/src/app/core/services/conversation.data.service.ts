import { Injectable } from '@angular/core';
import { Conversation } from '../classes/conversation.model';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root',
})
export class ConversationDataService {
  private userId!: string;
  private readonly defaultGroupImage = 'assets/logo.png';
  private readonly defaultUserImage = 'assets/user_logo.png';

  constructor(private authService: AuthService) {
    this.userId = authService.user()?.nameidentifier!;
  }

  getConversationTitle(conversation: Conversation): string {
    if (conversation.group) {
      return conversation.group.title;
    }

    const participant = this.getParticipant(conversation);
    return participant?.firstName + ' ' + participant?.lastName;
  }

  private getParticipant(conversation: Conversation) {
    const participant = conversation.privateConversationParticipants.find(
      (participant) => participant.id !== this.userId
    );
    return participant;
  }

  getConversationImage(conversation: Conversation) {
    if (conversation.group) {
      return this.getGroupImageSource(conversation);
    } 
    else {
      return this.getUserImageSource(conversation);
    }
  }

  private getGroupImageSource(conversation: Conversation): string {
    return conversation.group?.imgUrl || this.defaultGroupImage;
  }

  private getUserImageSource(conversation: Conversation): string {
    const userId = this.authService.user()?.nameidentifier;
    const participant = conversation.privateConversationParticipants.find(
      (participant) => participant.id !== userId
    );
    return participant?.imgUrl || this.defaultUserImage;
  }
}

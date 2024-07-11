import { Component } from '@angular/core';
import { Conversation } from '../../core/auth/classes/conversation.model';
import { ConversationsService } from '../../core/auth/services/conversations.service';

@Component({ 
  selector: 'app-conversations',
  templateUrl: './conversations.component.html',
  styleUrl: './conversations.component.css'
})
export class ConversationsComponent {
  conversations: Conversation[] = [];

  constructor(private conversationsService: ConversationsService) {}

  ngOnInit() {
    this.getConversations();
  }

  getConversations() {
    this.conversationsService.GetConversations().subscribe(
      (data) => {
        this.conversations = data; 
        console.log('Conversations:', this.conversations);
      },
      (error) => {
        console.error('Error fetching conversations:', error);
      }
    );
  }
}

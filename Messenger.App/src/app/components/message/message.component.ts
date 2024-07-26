import { Component, Input, OnInit } from '@angular/core';
import { Message } from '../../core/classes/message.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {
    @Input() myMessage = false;
    @Input() message!: Message;
    sentAt!: string;

    constructor(private authService: AuthService) {}

    ngOnInit(): void {
      if (this.message.sentAt) {
        this.sentAt = this.formatDateTime(this.message.sentAt.toString());
      }
    }
  
    formatDateTime(dateString: string): string {
      const date = new Date(dateString);
      return `${date.toLocaleDateString([], {
        year: 'numeric',
        month: 'short',
        day: 'numeric'
      })} ${date.toLocaleTimeString([], {
        hour: '2-digit',
        minute: '2-digit',
        hour12: true
      })}`;
    }

    getSenderDisplayText(): string {
      const userId = this.authService.user()?.nameidentifier;
  
      if (this.message !== null) {
        if (this.message.sender?.id === userId) {
          return '';
        } else {
          const firstName = this.message?.sender.firstName ?? '';
          const lastName = this.message?.sender.lastName ?? '';
          return `${firstName} ${lastName}`;
        }
      }
      return '';
    }
}

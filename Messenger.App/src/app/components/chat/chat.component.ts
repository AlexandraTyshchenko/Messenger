import { Component, OnInit, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { MessagesService } from '../../core/services/messages.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { Message } from '../../core/classes/message.model';
import { ActivatedRoute, ParamMap } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
})
export class ChatComponent implements OnInit  {
  conversationId: string | null = null;
  messages: Message[] = [];
  currentUserId!: string;
  currentPage = 1;
  itemsPerPage = 10;
  @ViewChild('messageInput') private messageInput!: ElementRef;

  constructor(
    private messagesService: MessagesService,
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.currentUserId = this.authService.user()?.nameidentifier!;
    this.route.queryParamMap.subscribe((params: ParamMap) => {
      const id = params.get('conversationId');
      if (id) {
        this.conversationId = id;
        this.loadMessages(this.conversationId);
        this.scrollToBottom()
      }
    });
  }

  appendData() {
    this.messagesService
      .GetMessages(this.conversationId!, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response) => {
          this.messages = [...this.messages, ...response.entities];
        },
        error: (err) => console.log(err),
      });
  }

  onScroll() {
    this.currentPage++;
    this.appendData();
    console.log('onscroll is invoked');

  }

  loadMessages(conversationId: string): void {
    this.messagesService
      .GetMessages(conversationId, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (pagedMessages: PagedEntities<Message>) => {
          this.messages = pagedMessages.entities;
          console.log(this.currentPage);
        },
        error: (error) => {
          console.error('Error loading messages:', error);
        },
      });
  }

  scrollToBottom(): void {
    const inputElement = this.messageInput.nativeElement;
    inputElement.scrollIntoView({ behavior: 'smooth' });
  }
}

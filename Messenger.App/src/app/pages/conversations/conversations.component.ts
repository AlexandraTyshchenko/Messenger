import { AfterViewInit, Component, OnInit, ViewChild } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { ConversationsService } from '../../core/services/conversations.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastDirective } from '../../directives/toast.directive';
import { SignalRService } from '../../core/services/signalr.service';
import { Message } from '../../core/classes/message.model';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-conversations',
  templateUrl: './conversations.component.html',
  styleUrls: ['./conversations.component.css'],
})
export class ConversationsComponent implements OnInit, AfterViewInit {
  conversations: Conversation[] = [];
  currentPage = 1;
  itemsPerPage = 10;
  selectedConversation: Conversation | null = null;
  selectedConversationId: string | null = null;
  conversationWithLatestMessage: Conversation | null = null;
  @ViewChild('toast', { static: true, read: ToastDirective })
  toast!: ToastDirective;
  latestMessage: Message | null = null;
  notificationMessage: string | null = null;

  constructor(
    private conversationsService: ConversationsService,
    private router: Router,
    private route: ActivatedRoute,
    private signalRService: SignalRService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.loadData();
    this.notificationMessage = null;
  }

  hideMessage() {
    this.toast.hide();
  }

  ngAfterViewInit(): void {
    this.signalRService.onNotificationReceive();
    this.signalRService.onJoinNotification();
    this.signalRService.joinNotification$.subscribe((message) => {
      if (message) {
        this.loadData();
        this.notificationMessage = message.text;
        this.handleMessage(message);
      }
    });

    this.signalRService.message$.subscribe((message) => {
      if (message) {
        this.conversationWithLatestMessage = this.conversations.find(
          (x) => x.id === message.conversationId
        )!;
        this.conversationWithLatestMessage!.lastMessage = message;
        this.handleMessage(message);
      }
    });
  }

  handleMessage(message: Message) {
    if (
      message.sender?.id !== this.authService.user()!.nameidentifier ||
      message.sender === null
    ) {
      this.toast.show();
    }
    console.log('Handling message:', message);
  }

  loadData() {
    this.conversationsService
      .GetConversations(this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: PagedEntities<Conversation>) => {
          this.conversations = response.entities;
          this.selectConversationFromRoute();
        },
        error: (err) => console.error('Error fetching conversations:', err),
      });
  }

  selectConversationFromRoute() {
    this.route.queryParams.subscribe((params) => {
      this.selectedConversationId = params['conversationId'];
      this.selectedConversation = this.conversations.find(
        (x) => x.id === this.selectedConversationId
      )!;
    });
  }

  appendData() {
    this.conversationsService
      .GetConversations(this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response) =>
          (this.conversations = [...this.conversations, ...response.entities]),
        error: (err) => console.log(err),
      });
  }

  onScroll() {
    this.currentPage++;
    this.appendData();
  }

  setActive(index: number) {
    this.selectedConversation = this.conversations[index];
    this.router.navigate([], {
      queryParams: { conversationId: this.selectedConversation.id },
      queryParamsHandling: 'merge',
    });
  }
}

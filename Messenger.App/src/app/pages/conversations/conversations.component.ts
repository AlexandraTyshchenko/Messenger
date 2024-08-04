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
    this.initializeSignalRSubscriptions();
  }
  
  private initializeSignalRSubscriptions(): void {
    this.signalRService.onNotificationReceive();
    this.signalRService.onJoinNotification();
    this.signalRService.onLeaveConversationNotification();
  
    this.subscribeToNotifications();
    this.subscribeToMessages();
  }
  
  private subscribeToNotifications(): void {
    this.signalRService.joinNotification$.subscribe(message => this.handleNotification(message));
    this.signalRService.leaveConversationNotification$.subscribe(message => this.handleNotification(message));
  }
  
  private subscribeToMessages(): void {
    this.signalRService.message$.subscribe(message => {
      if (message) {
        this.updateConversationWithMessage(message);
        this.handleMessage(message);
      }
    });
  }
  createPrivateConersation(){

  }
  createGroupConversation(){
    
  }
  private handleNotification(message: any): void {
    console.log(message)
    console.log(this.notificationMessage)
    if (message) {
      this.loadData();
      this.notificationMessage = message.text;
      this.handleMessage(message);
    }
  }
  
  private updateConversationWithMessage(message: any): void {
    this.conversationWithLatestMessage = this.conversations.find(
      conversation => conversation.id === message.conversationId
    )!;
    this.conversationWithLatestMessage!.lastMessage = message;
  }
  

  handleMessage(message: Message) {
    if (
      message.sender?.id !== this.authService.user()!.nameidentifier &&
      message.sender !== null
    ) {
      this.toast.show();
    }
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

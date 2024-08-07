import { AfterViewInit, Component, OnChanges, OnInit, SimpleChanges, ViewChild } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { ConversationsService } from '../../core/services/conversations.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { ActivatedRoute, Router } from '@angular/router';
import { ToastDirective } from '../../directives/toast.directive';
import { SignalRService } from '../../core/services/signalr.service';
import { Message } from '../../core/classes/message.model';
import { AuthService } from '../../core/services/auth.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { GroupConversationFormComponent } from '../../components/group-conversation-form/group-conversation-form.component';
import { SearchUsersComponent } from '../../components/search-users/search-users.component';

@Component({
  selector: 'app-conversations',
  templateUrl: './conversations.component.html',
  styleUrls: ['./conversations.component.css'],
})
export class ConversationsComponent implements OnInit, AfterViewInit,OnChanges {
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
  messagesCount:null|number=null
  isSidebarCollapsed = true;

  constructor(
    private conversationsService: ConversationsService,
    private router: Router,
    private route: ActivatedRoute,
    private signalRService: SignalRService,
    private authService: AuthService,
    private modalService: NgbModal
  ) {}

  ngOnInit() {
    this.loadData();
    this.notificationMessage = null;
  }

  hideMessage() {
    this.toast.hide();
  }

  toggleSidebar() {
    this.isSidebarCollapsed = !this.isSidebarCollapsed;
  }

  ngAfterViewInit(): void {
    this.initializeSignalRSubscriptions();
    
    this.conversationsService.conversation$.subscribe({
      next: () => this.loadData(),
      error: (err) => console.error('Error subscribing to conversation$: ', err)
    });
  }
  
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['conversation'] && !changes['conversation'].isFirstChange()) {
      this.currentPage = 1;
      this.loadData();
    }
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
    this.modalService.open(SearchUsersComponent, {
      size: 'lg',
    });
  }
  createGroupConversation(){
    this.modalService.open(GroupConversationFormComponent, {
      size: 'lg',
    });
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
    this.isSidebarCollapsed=false
    this.conversationsService
      .getConversations(this.currentPage, this.itemsPerPage)
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
      this.isSidebarCollapsed=true
      console.log(this.selectedConversationId)
      this.selectedConversation = this.conversations.find(
        (x) => x.id === this.selectedConversationId
      )!;
    });
    console.log(this.selectedConversation)
  }

  appendData() {
    this.conversationsService
      .getConversations(this.currentPage, this.itemsPerPage)
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
    this.isSidebarCollapsed=true
  }
}

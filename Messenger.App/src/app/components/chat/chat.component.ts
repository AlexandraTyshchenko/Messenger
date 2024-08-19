import {
  Component,
  OnInit,
  AfterViewInit,
  Input,
  ViewChild,
  ElementRef,
  OnChanges,
  SimpleChanges,
  Inject,
} from '@angular/core';
import { MessagesService } from '../../core/services/messages.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { Message } from '../../core/classes/message.model';
import { Conversation } from '../../core/classes/conversation.model';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MessageDto } from '../../core/classes/message-dto.model';
import { SignalRService } from '../../core/services/signalr.service';
import { AuthService } from '../../core/services/auth.service';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { AddParticipantsComponent } from '../add-participants/add-participants.component';
import { ParticipantsListComponent } from '../participants-list/participants-list.component';
import { LeaveConversationConfirmationModalComponent } from '../leave-conversation-confirmation-modal/leave-conversation-confirmation-modal.component';
import { FileUploadComponent } from '../file-upload/file-upload.component';

@Component({
  selector: 'app-chat',
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css'],
})
export class ChatComponent implements OnInit, AfterViewInit, OnChanges {
  @Input() conversation!: Conversation;
  messages: Message[] = [];
  currentUserId!: string;
  currentPage = 1;
  itemsPerPage = 10;
  @ViewChild('scrollContainer') private scrollContainer!: ElementRef;
  messageForm: FormGroup;
  @ViewChild(FileUploadComponent) fileUploadComponent!: FileUploadComponent;
  selectedFile: File | null = null;
  constructor(
    private messagesService: MessagesService,
    private fb: FormBuilder,
    private signalRService: SignalRService,
    private authService: AuthService,
    private modalService: NgbModal
  ) {
    this.messageForm = this.fb.group({
      message: ['', this.selectedFile ? [] : Validators.required], 
    });
  }

  ngOnInit(): void {
    this.currentUserId = this.authService.user()!.nameidentifier;
    if (this.conversation) {
      this.loadMessages(this.conversation.id);
    }
  }

  ngAfterViewInit(): void {
    this.signalRService.message$.subscribe((message) => {
      if (message) {
        console.log(message)
        this.addMessage(message);
      }
    });

    this.scrollToBottom();
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['conversation'] && !changes['conversation'].isFirstChange()) {
      this.currentPage = 1;
      this.messages = [];
      this.loadMessages(this.conversation.id);
    }
  }

  appendData() {
    this.messagesService
      .getMessages(this.conversation.id, this.currentPage, this.itemsPerPage)
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
    setTimeout(() => {
      this.scrollToCenter();
    }, 0);
    console.log('onscroll is invoked');
  }

  addParticipants() {
    this.modalService.open(AddParticipantsComponent, {
      size: 'lg',
    }).componentInstance.conversationId = this.conversation.id;
  }

  showParticipants() {
    this.modalService.open(ParticipantsListComponent, {
      size: 'lg',
    }).componentInstance.conversationId = this.conversation.id;
  }

  loadMessages(conversationId: string): void {
    this.messagesService
      .getMessages(conversationId, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (pagedMessages: PagedEntities<Message>) => {
          this.messages = pagedMessages.entities;
          setTimeout(() => {
            this.scrollToBottom();
          }, 0);
        },
        error: (error) => {
          console.error('Error loading messages:', error);
        },
      });
  }

  scrollToCenter() {
    if (!this.scrollContainer) {
      return;
    }
    const container = this.scrollContainer.nativeElement;
    container.scrollTop = container.scrollHeight * 0.1;
  }

  scrollToBottom(): void {
    if (!this.scrollContainer) {
      return;
    }
    const container = this.scrollContainer.nativeElement;
    container.scrollTop = container.scrollHeight;
  }

  sendMessage() {
    if (this.messageForm.valid ) {
      const message = this.messageForm.value.message;
      this.messagesService
        .sendMessage(
          this.conversation.id,
          new MessageDto(message, this.selectedFile)
        )
        .subscribe({
          next: (sentMessage: Message) => {
            setTimeout(() => {
              this.scrollToBottom();
            }, 0);
          },
          error: (error) => {
            console.error('Error loading messages:', error);
          },
        });

      this.fileUploadComponent.clearFileInput();
      this.messageForm.reset();
    }
  }

  onFileSelected(file: File | null) {
    this.selectedFile = file;
  }

  private addMessage(message: Message) {
    this.messages = [message, ...this.messages];
    if (this.messages.length > this.itemsPerPage) {
      this.messages.pop();
    }
    this.currentPage = 1;
  }

  leaveConversation() {
    const modalRef = this.modalService.open(
      LeaveConversationConfirmationModalComponent,
      {
        size: 'lg',
      }
    );

    modalRef.componentInstance.conversationId = this.conversation.id;
  }
}

import { Component, HostListener } from '@angular/core';
import { Conversation } from '../../core/classes/conversation.model';
import { ConversationsService } from '../../core/services/conversations.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-conversations',
  templateUrl: './conversations.component.html',
  styleUrls: ['./conversations.component.css'],
})
export class ConversationsComponent {
  conversations: Conversation[] = [];
  currentPage = 1;
  itemsPerPage = 10;
  selectedConversationId: string | null = null;

  constructor(
    private conversationsService: ConversationsService,
    private router: Router,
    private route: ActivatedRoute,
  ) {}

  ngOnInit() {
    this.loadData();
    this.route.queryParams.subscribe(params => {
      this.selectedConversationId = params['conversationId'];
    });

  }

  loadData() {
    this.conversationsService
      .GetConversations(this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: PagedEntities<Conversation>) => {
          this.conversations = response.entities;
        },
        error: (err) => console.error('Error fetching conversations:', err),
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
    console.log('onscroll is invoked');
  }

  setActive(index: number) {
    const selectedConversationId = this.conversations[index].id;
    this.router.navigate([], {
      queryParams: { conversationId: selectedConversationId },
      queryParamsHandling: 'merge',
    });
  }
}

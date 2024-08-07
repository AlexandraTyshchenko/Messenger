import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserInfo } from '../../core/classes/user-info.model';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { UserService } from '../../core/services/user.service';
import { PagedEntities } from '../../core/classes/pagination.model';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ConversationsService } from '../../core/services/conversations.service';
import { UserDto } from '../../core/classes/user-dto.model';
import { Conversation } from '../../core/classes/conversation.model';

@Component({
  selector: 'app-search-users',
  templateUrl: './search-users.component.html',
  styleUrls: ['./search-users.component.css']
})
export class SearchUsersComponent {
  searchForm: FormGroup;
  users: UserInfo[] = [];
  private searchSubject = new Subject<string>();
  currentPage = 1;
  itemsPerPage = 10;
  query: string = '';
  @Output() selectedUsersChange = new EventEmitter<UserInfo[]>(); 

  defaultImageUrl: string = "../../../assets/user_logo.png"; 

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
    public activeModal: NgbActiveModal,
    private conversationsService:ConversationsService
  ) {
    this.searchForm = this.fb.group({
      searchQuery: [''],
    });
  }

  ngOnInit(): void {
    this.searchUsers();
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((query) => {
        this.query = query;
        this.currentPage = 1; 
        this.searchUsers();
      });

    this.searchForm.get('searchQuery')?.valueChanges.subscribe((value) => {
      this.searchSubject.next(value);
    });

    if (this.query) {
      this.searchUsers();
    }
  }

  searchUsers() {
    this.userService.getUsers(this.query, this.currentPage, this.itemsPerPage).subscribe({
      next: (response: PagedEntities<UserInfo>) => {
        this.users = response.entities;        
      },
      error: (err) => {
        console.error('Error fetching users:', err);
      },
    });
  }


  appendData() {
    this.userService.getUsers(this.query, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: PagedEntities<UserInfo>) =>
          (this.users = [...this.users, ...response.entities]),
        error: (err) => console.error('Error fetching additional users:', err),
      });
  }

  createPrivateConversation(id: string) {
    const userDto = new UserDto(id);

    this.conversationsService.createPrivateConversation(userDto).subscribe({
      next: (response:Conversation) =>
        this.activeModal.close(),
      error: (err) => console.error('Error fetching additional users:', err),
    });
}


  onScroll() {
    this.currentPage++;
    this.appendData();
  }
}

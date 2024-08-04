import { Component, Input, Output, EventEmitter } from '@angular/core';
import { UserInfo } from '../../core/classes/user-info.model';
import { FormBuilder, FormGroup } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { UserService } from '../../core/services/user.service';
import { PagedEntities } from '../../core/classes/pagination.model';

@Component({
  selector: 'app-search-users',
  templateUrl: './search-users.component.html',
  styleUrls: ['./search-users.component.css']
})
export class SearchUsersComponent {
  @Input() filterFunction: ((user: UserInfo) => boolean) | null = null;
  searchForm: FormGroup;
  users: UserInfo[] = [];
  private searchSubject = new Subject<string>();
  selectedUsers: UserInfo[] = []; 
  currentPage = 1;
  itemsPerPage = 10;
  query: string = '';
  @Output() selectedUsersChange = new EventEmitter<UserInfo[]>(); 

  defaultImageUrl: string = "../../../assets/user_logo.png"; 

  constructor(
    private fb: FormBuilder,
    private userService: UserService,
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
        if (this.filterFunction) {
          this.users = this.users.filter(this.filterFunction);
        }
      },
      error: (err) => {
        console.error('Error fetching users:', err);
      },
    });
  }

  select(user: UserInfo) {
    const index = this.selectedUsers.findIndex(x => x.id === user.id);
    if (index > -1) {
      this.selectedUsers.splice(index, 1);
    } else {
      this.selectedUsers.push(user);
    }
    this.selectedUsersChange.emit(this.selectedUsers); 
    console.log(this.selectedUsers);
  }

  isSelected(user: UserInfo): boolean {
    return this.selectedUsers.some(x => x.id === user.id);
  }

  appendData() {
    this.userService.getUsers(this.query, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: PagedEntities<UserInfo>) =>
          (this.users = [...this.users, ...response.entities]),
        error: (err) => console.error('Error fetching additional users:', err),
      });
  }

  onScroll() {
    this.currentPage++;
    this.appendData();
  }
}

import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserInfo } from '../../core/classes/user-info.model';
import { ParticipantService } from '../../core/services/participant.service';
import { ParticipantsInConversationDto } from '../../core/classes/participants-in-conversation.model';
import { Participant } from '../../core/classes/participant.model';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { FormBuilder, FormGroup } from '@angular/forms';
import { UserService } from '../../core/services/user.service';
import { PagedEntities } from '../../core/classes/pagination.model';

@Component({
  selector: 'app-add-participants',
  templateUrl: './add-participants.component.html',
  styleUrls: ['./add-participants.component.css'],
})
export class AddParticipantsComponent implements OnInit {
  @Input() conversationId!: string;
  errorMessage: string | null = null;
  participants: Participant[] = [];
  searchForm: FormGroup;
  users: UserInfo[] = [];
  private searchSubject = new Subject<string>();
  selectedUsers: UserInfo[] = [];
  currentPage = 1;
  itemsPerPage = 10;
  query: string = '';

  defaultImageUrl: string = '../../../assets/user_logo.png';
  constructor(
    private participantService: ParticipantService,
    public activeModal: NgbActiveModal,
    private fb: FormBuilder,
    private userService: UserService
  ) {
    this.searchForm = this.fb.group({
      searchQuery: [''],
    });
  }

  ngOnInit(): void {
    this.searchUsers();
    this.loadParticipants();
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((query) => {
        this.query = query;
        this.currentPage = 1;
        this.searchUsers();
        this.errorMessage = null;
      });

    this.searchForm.get('searchQuery')?.valueChanges.subscribe((value) => {
      this.searchSubject.next(value);
    });

    if (this.query) {
      this.searchUsers();
    }
  }

  isParticipant(user: UserInfo): boolean {
    const participant = this.participants.find(
      (participant) => participant.userInfo.id === user.id
    );
    if (participant) {
      return true;
    }
    return false;
  }

  loadParticipants(): void {
    this.participantService.getAllParticipants(this.conversationId).subscribe({
      next: (response: Participant[]) => {
        this.participants = response;
        this.errorMessage = null;
      },
      error: (error: any) => {
        this.errorMessage =
          'Failed to load participants. Please try again later.';
        console.error('Error loading participants:', error);
      },
    });
  }

  submit() {
    const userIds = Array.from(this.selectedUsers).map((user) => user.id);

    this.participantService
      .addParticipant(this.conversationId, userIds)
      .subscribe({
        next: (response: ParticipantsInConversationDto) => {
          this.close();
        },
        error: (err: any) => {
          this.errorMessage = err;
        },
      });
  }

  onSelectedUsersChange(users: UserInfo[]): void {
    this.selectedUsers = users;
  }

  close() {
    this.activeModal.close();
  }

  searchUsers() {
    this.userService
      .getUsers(this.query, this.currentPage, this.itemsPerPage)
      .subscribe({
        next: (response: PagedEntities<UserInfo>) => {
          this.users = response.entities;
        },
        error: (err) => {
          console.error('Error fetching users:', err);
        },
      });
  }

  select(user: UserInfo) {
    const index = this.selectedUsers.findIndex((x) => x.id === user.id);
    if (index > -1) {
      this.selectedUsers.splice(index, 1);
    } else {
      this.selectedUsers.push(user);
    }
  }

  isSelected(user: UserInfo): boolean {
    return this.selectedUsers.some((x) => x.id === user.id);
  }

  appendData() {
    this.userService
      .getUsers(this.query, this.currentPage, this.itemsPerPage)
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

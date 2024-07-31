import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { UserInfo } from '../../core/classes/user-info.model';
import { ParticipantService } from '../../core/services/participant.service';
import { ParticipantsInConversationDto } from '../../core/classes/participants-in-conversation.model';

@Component({
  selector: 'app-add-participants',
  templateUrl: './add-participants.component.html',
  styleUrls: ['./add-participants.component.css'],
})
export class AddParticipantsComponent implements OnInit {
  @Input() conversationId!: string;
  selectedUsers: Set<UserInfo> = new Set();

  constructor(private participantService: ParticipantService, public activeModal: NgbActiveModal,
  ) {}

  ngOnInit(): void {}

  submit() {
    const userIds = Array.from(this.selectedUsers).map((user) => user.id);

    this.participantService
      .addParticipant(this.conversationId, userIds)
      .subscribe({
        next: (response: ParticipantsInConversationDto) => {
          console.log('Participants added successfully:', response);
          this.close();
        },
        error: (err) => {
          console.error('Error adding participants:', err);
        },
      });
  }

  onSelectedUsersChange(users: Set<UserInfo>): void {
    this.selectedUsers = users;
    console.log('Selected Users:', this.selectedUsers);
  }

  close() {
    this.activeModal.close();
  }
}

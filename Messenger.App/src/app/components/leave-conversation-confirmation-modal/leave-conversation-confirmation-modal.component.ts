import { Component, Input } from '@angular/core';
import { ParticipantService } from '../../core/services/participant.service';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-leave-conversation-confirmation-modal',
  templateUrl: './leave-conversation-confirmation-modal.component.html',
  styleUrl: './leave-conversation-confirmation-modal.component.css',
})
export class LeaveConversationConfirmationModalComponent {
  @Input() conversationId: string | null = null;
  constructor(
    private participantService: ParticipantService,
    public activeModal: NgbActiveModal
  ) {}

  leaveConversation(): void {
    if (this.conversationId) {
      this.participantService.leaveConversation(this.conversationId).subscribe({
        next: (response: any) => {
          this.activeModal.close();
        },
        error: (error: any) => {
          console.error('Error leaving conversation', error);
        },
      });
    }
  }
}

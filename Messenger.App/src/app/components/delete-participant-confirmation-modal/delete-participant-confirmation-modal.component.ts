import { Component, Input } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { Participant } from '../../core/classes/participant.model';
import { ParticipantService } from '../../core/services/participant.service';

@Component({
  selector: 'app-delete-participant-confirmation-modal',
  templateUrl: './delete-participant-confirmation-modal.component.html',
  styleUrl: './delete-participant-confirmation-modal.component.css',
})
export class DeleteParticipantConfirmationModalComponent {
  @Input() participant: Participant | null = null;
  @Input() conversationId: string | null = null;
  constructor(public activeModal: NgbActiveModal,
    private participantService:ParticipantService
  ) {}

  deleteParticipant(): void {
    if (this.participant && this.conversationId) {
      this.participantService.deleteParticipant(this.conversationId, this.participant.userInfo.id)
        .subscribe({
          next: (response: any) => {
            this.activeModal.close();
          },
          error: (error: any) => {
            console.error('Error deleting participant:', error);
          }
        });
    }
  }  
}

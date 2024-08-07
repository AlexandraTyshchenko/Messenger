import { Component, Input, OnInit } from '@angular/core';
import { NgbActiveModal, NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { ParticipantService } from '../../core/services/participant.service';
import { UserInfo } from '../../core/classes/user-info.model';
import { AuthService } from '../../core/services/auth.service';
import { Participant, Role } from '../../core/classes/participant.model';
import { DeleteParticipantConfirmationModalComponent } from '../delete-participant-confirmation-modal/delete-participant-confirmation-modal.component';

@Component({
  selector: 'app-participants-list',
  templateUrl: './participants-list.component.html',
  styleUrls: ['./participants-list.component.css'],
})
export class ParticipantsListComponent implements OnInit {
  @Input() conversationId!: string;
  participants: Participant[] = [];
  errorMessage: string | null = null;
  defaultImageUrl: string = '../../../assets/user_logo.png';
  userId: string | null = null;
  isAdmin = false;
  constructor(
    public activeModal: NgbActiveModal,
    private participantService: ParticipantService,
    private authService: AuthService,
    private modalService: NgbModal,
  ) {}

  ngOnInit(): void {
    this.loadParticipants();
    this.userId = this.authService.user()!.nameidentifier;
    this.participantService.participantDeleted$.subscribe({
      next:(response:any)=>{
        this.loadParticipants();
      },
      error:(error:any)=>{
        console.log(error);
      }
    })
  }

  loadParticipants(): void {
    this.participantService.getAllParticipants(this.conversationId).subscribe({
      next: (response: Participant[]) => {
        this.participants = response;
        this.errorMessage = null;
        this.checkPermissions();
      },
      error: (error: any) => {
        this.errorMessage =
          'Failed to load participants. Please try again later.';
        console.error('Error loading participants:', error);
      },
    });
  }

  checkPermissions() {
    const participant = this.participants.find((x) => x.userInfo.id == this.userId);
    if (participant?.role === Role.Admin) {
      this.isAdmin = true;
      console.log(this.participants);
    }
  }
  close(): void {
    this.activeModal.close();
  }

  deleteParticipant(participant: Participant): void {
    const modalRef = this.modalService.open(DeleteParticipantConfirmationModalComponent, {
      size: 'lg',
    });
  
    modalRef.componentInstance.conversationId = this.conversationId;
    modalRef.componentInstance.participant = participant;
  }
  
}

import { Component } from '@angular/core';
import { NgbActiveModal } from '@ng-bootstrap/ng-bootstrap';
import { ConversationsService } from '../../core/services/conversations.service';
import { GroupDto } from '../../core/classes/group-dto.model';

@Component({
  selector: 'app-group-conversation-form',
  templateUrl: './group-conversation-form.component.html',
  styleUrls: ['./group-conversation-form.component.css'] 
})
export class GroupConversationFormComponent {
  title: string = '';
  imageFile: string | null = null;

  constructor(
    public activeModal: NgbActiveModal,
    private conversationsService: ConversationsService
  ) {}

  createGroup() {
    const groupDto: GroupDto = {
      title: this.title,
      imgUrl: this.imageFile ? this.imageFile : null
    };

    this.conversationsService.createGroupConversation(groupDto).subscribe({
      next: (response) => {
        console.log(groupDto)
        this.activeModal.close(); 
      },
      error: (err) => {
        console.log(groupDto)

        console.error('Error creating group:', err);
      }
    });
  }

  onFileChange(event: Event) {
      this.imageFile = null;
  }
}

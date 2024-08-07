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
        console.log('Group created successfully', groupDto);
        this.activeModal.close(); 
      },
      error: (err) => {
        console.error('Error creating group:', err);
      }
    });
  }

  onFileChange(event: Event) {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const file = input.files[0];
      const reader = new FileReader();

      reader.onload = (e: ProgressEvent<FileReader>) => {
        if (e.target) {
          this.imageFile = e.target.result as string;
        }
      };

      reader.readAsDataURL(file);
    }
  }
}

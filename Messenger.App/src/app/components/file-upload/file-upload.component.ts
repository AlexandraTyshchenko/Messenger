import { Component, EventEmitter, Input, Output } from '@angular/core';
import { ImageService } from '../../core/services/image.service';
import { Image } from '../../core/classes/image.model';
import { ImageResult } from '../../core/classes/image.result.model';

interface FileWithPreview extends File {
  preview?: string;
}

@Component({
  selector: 'app-file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css'],
})
export class FileUploadComponent {
  @Output() fileSelected = new EventEmitter<FileWithPreview | null>();
  @Input() messageText: string | null = null;
  @Input() conversationId: string | null = null;
  selectedFile: FileWithPreview | null = null;

  constructor(private imageService: ImageService) {}

  onFileSelected(event: any) {
    const file = event.target.files[0] as FileWithPreview;

    if (file && file.type.startsWith('image/')) {
      const reader = new FileReader();
      reader.onload = () => {
        file.preview = reader.result as string;
        this.selectedFile = file;
        this.fileSelected.emit(this.selectedFile);
      };
      reader.readAsDataURL(file);
    } else {
      this.selectedFile = null;
      this.fileSelected.emit(null);
    }
  }

  uploadFile() {
    if (this.selectedFile && this.conversationId) {
      this.imageService.sendImage(
        this.conversationId,
        new Image(this.selectedFile, this.messageText || '')
      ).subscribe({
        next: (response: ImageResult) => {
          console.log(response);
          this.clearFileInput();
        },
        error: (err: any) => {
          console.log(err);
        }
      });
    }
  }

  triggerFileUpload() {
    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.click();
    }
  }

  private clearFileInput() {
    this.selectedFile = null;
    this.fileSelected.emit(null);

    const fileInput = document.getElementById('fileInput') as HTMLInputElement;
    if (fileInput) {
      fileInput.value = '';
    }
  }
}

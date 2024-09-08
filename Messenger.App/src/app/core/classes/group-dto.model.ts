export class GroupDto {
    title: string = '';
    img: File | null;
  
    constructor(title: string, img: File | null) {
      this.title = title;
      this.img = img;
    }
  }
  
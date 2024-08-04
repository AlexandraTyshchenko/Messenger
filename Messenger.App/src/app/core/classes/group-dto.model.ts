export class GroupDto {
    title: string = '';
    imgUrl: string | null;
  
    constructor(title: string, imgUrl: string | null) {//todo change to IFile type
      this.title = title;
      this.imgUrl = imgUrl;
    }
  }
  
import { Guid } from "guid-typescript";

export class Group {
  id: Guid;
  title: string = '';
  imgUrl: string = '';

  constructor(id: Guid, title: string, imgUrl: string) {
    this.id = id;
    this.title = title;
    this.imgUrl = imgUrl;
  }
}

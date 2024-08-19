export class MessageDto {
  text: string;
  image: File|null;
  constructor(text: string, image: File|null) {
    this.text = text;
    this.image = image;
  }
}

export class MessageWithSender {
  id: string; 
  senderUserName: string;
  senderFirstName: string;
  senderLastName: string;
  senderPhoneNumber: string;
  imgUrl: string;
  text: string;
  sentAt: Date;
  updatedAt: Date;

  constructor(
    id: string,
    senderUserName: string,
    senderFirstName: string,
    senderLastName: string,
    senderPhoneNumber: string,
    imgUrl: string,
    text: string,
    sentAt: Date,
    updatedAt: Date
  ) {
    this.id = id;
    this.senderUserName = senderUserName;
    this.senderFirstName = senderFirstName;
    this.senderLastName = senderLastName;
    this.senderPhoneNumber = senderPhoneNumber;
    this.imgUrl = imgUrl;
    this.text = text;
    this.sentAt = sentAt;
    this.updatedAt = updatedAt;
  }
}

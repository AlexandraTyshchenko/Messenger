import { ImageInfo } from './image-info.model';
import { UserInfo } from './user-info.model';

export class Message {
  public id: string;
  public sender: UserInfo|null;
  public text: string;
  public sentAt: Date;
  public updatedAt: Date;
  public conversationId:string;
  public isJoinMessage:boolean;
  public image:ImageInfo|null;
  constructor(
    id: string,
    sender: UserInfo,
    text: string,
    sentAt: Date,
    updatedAt: Date,
    conversationId:string,
    isJoinMessage:boolean,
    image:ImageInfo|null
  ) {
    this.id = id;
    this.sender = sender;
    this.text = text;
    this.sentAt = sentAt;
    this.updatedAt = updatedAt;
    this.conversationId = conversationId;
    this.isJoinMessage = isJoinMessage;
    this.image = image
  }
}

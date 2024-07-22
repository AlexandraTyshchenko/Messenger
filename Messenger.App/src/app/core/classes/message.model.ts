import { UserInfo } from './user-info.model';

export class Message {
  public id: string;
  public sender: UserInfo;
  public text: string;
  public sentAt: Date;
  public updatedAt: Date;

  constructor(
    id: string,
    sender: UserInfo,
    text: string,
    sentAt: Date,
    updatedAt: Date
  ) {
    this.id = id;
    this.sender = sender;
    this.text = text;
    this.sentAt = sentAt;
    this.updatedAt = updatedAt;
  }
}

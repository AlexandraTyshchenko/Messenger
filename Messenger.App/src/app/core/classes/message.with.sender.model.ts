import { UserInfo } from './user-info.model';

export class MessageWithSender {
  id: string;
  sender: UserInfo;
  sentAt: Date;
  updatedAt: Date;
  text: string;
  constructor(
    id: string,
    sender: UserInfo,
    sentAt: Date,
    updatedAt: Date,
    text: string
  ) {
    this.id = id;
    this.sender = sender;
    this.sentAt = sentAt;
    this.updatedAt = updatedAt;
    this.text = text;
  }
}

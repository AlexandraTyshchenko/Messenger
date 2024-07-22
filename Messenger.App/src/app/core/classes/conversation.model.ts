import { UserInfo } from './user-info.model';
import { Group } from './group.model';
import { MessageWithSender } from './message.with.sender.model';
import { Guid } from 'guid-typescript';

export class Conversation {
  id: string;
  group: Group;
  lastMessage: MessageWithSender | null;
  participants: UserInfo[];
  constructor(
    id: string,
    group: Group,
    lastMessage: MessageWithSender | null,
    participants: UserInfo[]
  ) {
    this.id = id;
    this.group = group;
    this.lastMessage = lastMessage;
    this.participants = participants;
  }
}

import { UserInfo } from './user-info.model';
import { Group } from './group.model';
import { Message } from './message.model';

export class Conversation {
  id: string;
  group: Group;
  lastMessage: Message | null;
  privateConversationParticipants: UserInfo[];
  constructor(
    id: string,
    group: Group,
    lastMessage: Message | null,
    privateConversationParticipants: UserInfo[]
  ) {
    this.id = id;
    this.group = group;
    this.lastMessage = lastMessage;
    this.privateConversationParticipants = privateConversationParticipants;
  }
}

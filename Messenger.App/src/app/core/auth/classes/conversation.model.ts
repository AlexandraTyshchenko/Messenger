import { Group } from "./group.model";
import { MessageWithSender } from "./message.with.sender.model";
import { Guid } from 'guid-typescript';

export class Conversation {
    id: Guid;
    group: Group;
    lastMessage: MessageWithSender | null;
  
    constructor(id: Guid, group: Group, lastMessage: MessageWithSender | null) {
      this.id = id;
      this.group = group;
      this.lastMessage = lastMessage;
    }
  }
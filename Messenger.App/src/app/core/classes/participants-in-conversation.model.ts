import { UserInfo } from "./user-info.model";

export class ParticipantsInConversationDto {
    conversationId: string;
    participants: UserInfo[];
  
    constructor(conversationId: string, participants: UserInfo[]) {
      this.conversationId = conversationId;
      this.participants = participants;
    }
  }
  

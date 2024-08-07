import { UserInfo } from "./user-info.model";
export enum Role {
    Participant,
    Admin
}
export class Participant {
    userInfo: UserInfo;
    conversationId: string;
    role?: Role;

    constructor(userInfo: UserInfo, conversationId: string, role?: Role) {
        this.userInfo = userInfo;
        this.conversationId = conversationId;
        this.role = role;
    }
}

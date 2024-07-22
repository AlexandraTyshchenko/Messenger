import { Guid } from 'guid-typescript';

export class UserInfo {
  id: string;
  userName: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber: string;
  bio: string;
  imgUrl: string;

  constructor(
    id: string,
    userName: string,
    firstName: string,
    lastName: string,
    email: string,
    phoneNumber: string,
    bio: string,
    imgUrl: string
  ) {
    this.id = id;
    this.userName = userName;
    this.firstName = firstName;
    this.lastName = lastName;
    this.email = email;
    this.phoneNumber = phoneNumber;
    this.bio = bio;
    this.imgUrl = imgUrl;
  }
}

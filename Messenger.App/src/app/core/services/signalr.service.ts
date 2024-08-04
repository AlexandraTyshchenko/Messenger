import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { environment } from '../../../environments/environment.development';
import { Message } from '../classes/message.model';

@Injectable({
  providedIn: 'root',
})
export class SignalRService {
  private hubConnection: signalR.HubConnection;
  private messageSubject = new BehaviorSubject<Message | null>(null);
  private joinNotificationSubject = new BehaviorSubject<Message | null>(null);
  private leaveConversationNotification = new BehaviorSubject<Message | null>(
    null
  );

  constructor() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.apiUrl}chathub`, {
        accessTokenFactory: () => {
          const token = localStorage.getItem('token');
          return token ? token : '';
        },
      })
      .withAutomaticReconnect()
      .build();
  }

  public startConnection(): void {
    this.hubConnection
      .start()
      .then(() => {
        console.log('SignalR connection started.');
        this.onjoinGroups();
      })
      .catch((err) =>
        console.log('Error while starting SignalR connection: ' + err)
      );
  }

  onjoinGroups(): void {
    this.hubConnection
      .invoke('JoinGroups')
      .then(() => console.log('Successfully joined groups.'))
      .catch((err) => console.error('Error joining groups: ', err));
  }

  public onNotificationReceive(): void {
    this.hubConnection.on('ReceiveNotification', (message: Message) => {
      this.messageSubject.next(message);
    });
  }

  public onJoinNotification(): void {
    this.hubConnection.on('JoinNotification', (message: Message) => {
      this.joinNotificationSubject.next(message);
    });
  }

  public onLeaveConversationNotification(): void {
    this.hubConnection.on('LeaveConversationNotification', (message: Message) => {
      this.joinNotificationSubject.next(message);
    });
  }

  get message$(): Observable<Message | null> {
    return this.messageSubject.asObservable();
  }

  get joinNotification$(): Observable<Message | null> {
    return this.joinNotificationSubject.asObservable();
  }

  get leaveConversationNotification$(): Observable<Message | null> {
    return this.leaveConversationNotification.asObservable();
  }
}

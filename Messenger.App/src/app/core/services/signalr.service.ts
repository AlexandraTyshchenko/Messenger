import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { BehaviorSubject, Observable } from 'rxjs';
import { filter } from 'rxjs/operators';
import { environment } from '../../../environments/environment.development';
import { Message } from '../classes/message.model';
import { AuthService } from './auth.service';
import { ISignalRService } from '../interfaces/signalr.interface';

@Injectable({
  providedIn: 'root',
})
export class SignalRService implements ISignalRService {
  private hubConnection: signalR.HubConnection;
  private messageSubject = new BehaviorSubject<Message | null>(null);

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

  get message$(): Observable<Message | null>{
    return this.messageSubject.asObservable();
  }

}

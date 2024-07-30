import { InjectionToken } from '@angular/core';
import { ISignalRService } from './interfaces/signalr.interface';
import { IAuthService } from './interfaces/auth.interface';
import { IMessagesService } from './interfaces/message.interface';

export const ISignalRServiceToken = new InjectionToken<ISignalRService>('ISignalRService');
export const IAuthServiceToken = new InjectionToken<IAuthService>('IAuthService');
export const IMessagesServiceToken = new InjectionToken<IMessagesService>('IMessagesService');

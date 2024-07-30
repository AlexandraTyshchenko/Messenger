import { Observable } from 'rxjs';
import { Login } from '../auth/login/interfaces';
import { LoginResponse } from '../auth/login/types/login-response.type';
import { UserRegistration } from '../auth/login/classes/register-user.class';
import { User } from '../../interfaces/user.interface';
import { WritableSignal } from '@angular/core';

export interface IAuthService {
  user: WritableSignal<User | null>;
  isAuthenticated(): boolean;
  login(body: Login): Observable<LoginResponse>;
  register(body: UserRegistration): void;
  logout(): void;
  confirm(email: string, token: string): void;
  refreshToken(): Observable<LoginResponse | null>;
}

import {
  Injectable,
  inject,
  signal,
  WritableSignal,
} from '@angular/core';
import { HttpClient, HttpContext, HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, of, throwError } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';
import { JwtHelperService } from '@auth0/angular-jwt';
import { environment } from '../../../environments/environment.development';
import { Login } from '../auth/login/interfaces';
import { LoginResponse } from '../auth/login/types/login-response.type';
import { LoginSuccess } from '../auth/login/interfaces';
import { IS_PUBLIC } from '../interceptors/auth.interceptor';
import { RefreshToken } from '../auth/login/classes/refresh-token.class';
import { UserRegistration } from '../auth/login/classes/register-user.class';
import { User } from '../../interfaces/user.interface';

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly jwtHelper = inject(JwtHelperService);
  private readonly CONTEXT = {
    context: new HttpContext().set(IS_PUBLIC, true),
  };

  get user(): WritableSignal<User | null> {
    const token = localStorage.getItem('token');
    const decodedJWT = token ? JSON.parse(window.atob(token.split('.')[1])) : null;
    
    if (decodedJWT) {
      const user: User = {
        emailaddress: decodedJWT["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress"],
        name: decodedJWT["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
        nameidentifier: decodedJWT["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
        imgUrl: decodedJWT["ImgUrl"]
      };
      return signal(user);
    }
    
    return signal(null);
  }

  isAuthenticated(): boolean {
    return !this.jwtHelper.isTokenExpired();
  }

  login(body: Login): Observable<LoginResponse> {
    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}api/Auth/login`,
        body,
        this.CONTEXT
      )
      .pipe(
        tap((data) => {
          const loginSuccessData = data as LoginSuccess;
          this.storeTokens(loginSuccessData);
          this.router.navigate(['/']);
        }),
        catchError((error) => {
          if (error.status === 400) {
            return throwError(() => error.error);
          }
          return throwError(() => error);
        })
      );
  }

  register(body: UserRegistration) {
    return this.http
      .post(`${environment.apiUrl}api/Auth/register`, body, this.CONTEXT)
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error.error));
        })
      );
  }

  logout(){
    localStorage.removeItem('token');
    localStorage.removeItem('refresh_token');
    this.router.navigate(['/login']);
  }

  confirm(email: string, token: string) {
    const params = new HttpParams()
      .set('email', email)
      .set('token', token);
  
    return this.http
      .post(`${environment.apiUrl}api/Auth/confirmEmail`, {}, { params })
      .pipe(
        catchError((error) => {
          return throwError(() => new Error(error.error));
        })
      );
  }
  
  storeTokens(data: LoginSuccess): void {
    console.log('store tokens is invoked');
    localStorage.setItem('token', data.token);
    localStorage.setItem('refresh_token', data.refreshToken);
  }

  refreshToken(): Observable<LoginResponse | null> {
    const refresh_token = localStorage.getItem('refresh_token');
    if (!refresh_token || this.isAuthenticated()) {
      return of(null);
    }

    return this.http
      .post<LoginResponse>(
        `${environment.apiUrl}api/Auth/refresh`,
        new RefreshToken(refresh_token),
        this.CONTEXT
      )
      .pipe(
        catchError(() => {
          console.log('invalid refresh token');
          return of(null);
        }),
        tap((data) => {
          if (data) {
            const loginSuccessData = data as LoginSuccess;
            this.storeTokens(loginSuccessData);
          }
        })
      );
  }
}

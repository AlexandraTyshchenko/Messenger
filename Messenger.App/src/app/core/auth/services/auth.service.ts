import { DestroyRef, Injectable, inject, signal, WritableSignal } from '@angular/core';
import { HttpClient, HttpContext } from "@angular/common/http";
import { Router } from "@angular/router";
import { Observable, of } from "rxjs";
import { catchError, tap } from 'rxjs/operators';
import { JwtHelperService } from "@auth0/angular-jwt";
import { environment } from '../../../../environments/environment.development';
import { Login } from "../login/interfaces";
import { User } from "../../../interfaces/user.interface";
import { LoginResponse } from "../login/types/login-response.type";
import { LoginSuccess } from "../login/interfaces";
import { IS_PUBLIC } from "../auth.interceptor";
import { takeUntilDestroyed } from "@angular/core/rxjs-interop";

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);
  private readonly jwtHelper = inject(JwtHelperService);
  private readonly destroyRef = inject(DestroyRef);
  private readonly CONTEXT = { context: new HttpContext().set(IS_PUBLIC, true) };
  private readonly TOKEN_EXPIRY_THRESHOLD_MINUTES = 5;

  get user(): WritableSignal<User | null> {
    const token = localStorage.getItem('token');
    return signal(token ? this.jwtHelper.decodeToken(token) : null);
  }

  isAuthenticated(): boolean {
    return !this.jwtHelper.isTokenExpired();
  }

  login(body: Login): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/Auth/login`, body, this.CONTEXT)
      .pipe(
        catchError(error => {
          if (error.status === 400) {
            // Handle invalid credentials
            console.log('Invalid credentials');
          }
          return of();
        }),
        tap(data => {
          const loginSuccessData = data as LoginSuccess;
          this.storeTokens(loginSuccessData);
          this.scheduleTokenRefresh(loginSuccessData.token);
          this.router.navigate(['/']);
        })
      );
  }

  logout(): void {
    const refresh_token = localStorage.getItem('refresh_token');
    this.http.post<LoginResponse>(`${environment.apiUrl}/token/invalidate`, { refresh_token }, this.CONTEXT)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe(() => {
        localStorage.removeItem('token');
        localStorage.removeItem('refresh_token');
        this.router.navigate(['/login']);
      });
  }

  storeTokens(data: LoginSuccess): void {
    localStorage.setItem('token', data.token);
    localStorage.setItem('refresh_token', data.refresh_token);
  }

  refreshToken(): Observable<LoginResponse | null> {
    const refresh_token = localStorage.getItem('refresh_token');
    if (!refresh_token) {
      return of(null);
    }

    return this.http.post<LoginResponse>(
      `${environment.apiUrl}/Auth/refresh`, { refresh_token }, this.CONTEXT)
      .pipe(
        catchError(() => of(null)),
        tap(data => {
          const loginSuccessData = data as LoginSuccess;
          this.storeTokens(loginSuccessData);
          this.scheduleTokenRefresh(loginSuccessData.token);
        })
      );
  }

  scheduleTokenRefresh(token: string): void {
    const expirationTime = this.jwtHelper.getTokenExpirationDate(token)?.getTime();
    const refreshTime = expirationTime ? expirationTime - this.TOKEN_EXPIRY_THRESHOLD_MINUTES * 60 * 1000 : Date.now();
    const refreshInterval = refreshTime - Date.now();

    if (refreshInterval > 0) {
      setTimeout(() => {
        this.refreshToken()
          .pipe(takeUntilDestroyed(this.destroyRef))
          .subscribe();
      }, refreshInterval);
    }
  }
}

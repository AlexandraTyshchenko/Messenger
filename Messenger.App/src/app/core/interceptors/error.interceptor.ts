import {
  HttpInterceptorFn,
  HttpErrorResponse,
  HttpStatusCode,
} from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';
import { LoginResponse } from '../auth/login/types/login-response.type';
import { LoginSuccess } from '../auth/login/interfaces';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const authApiService = inject(AuthService);

  return next(req).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === HttpStatusCode.Unauthorized) {
        console.log('Unauthorized error detected');

        return authApiService.refreshToken()?.pipe(
          switchMap((resp: LoginResponse | null) => {
            if (resp && 'token' in resp) {
              const clonedRequest = req.clone({
                setHeaders: {
                  Authorization: `Bearer ${(resp as LoginSuccess).token}`,
                },
              });
              console.log('Token refreshed and request retried');

              return next(clonedRequest);
            }
            authApiService.logout();
            return throwError(
              () => new Error('User logged out due to unauthorized access')
            );
          })
        );
      }

      if (error.error && error.error.errors) {
        const errors = error.error.errors;
        for (let i = 0; i < errors.length; i++) {
          console.error(errors[i].message);
        }
        return throwError(
          () =>
            new Error(
              errors.map((e: { message: string }) => e.message).join(', ')
            )
        );
      }

      return throwError(() => new Error(error.message));
    })
  );
};

import { HttpContextToken, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authSvc = inject(AuthService);

  if (req.context.get(IS_PUBLIC)) {
    return next(req);
  }
  if (authSvc.isAuthenticated()) {
    const authRequest = addAuthorizationHeader(req);
    return next(authRequest);
  } else {
    return next(req);
  }
};
const addAuthorizationHeader = (req: HttpRequest<any>) => {
  const token = localStorage.getItem('token');
  return req.clone({
    headers: req.headers.set('Authorization', `Bearer ${token}`),
  });
};
export const IS_PUBLIC = new HttpContextToken(() => false);

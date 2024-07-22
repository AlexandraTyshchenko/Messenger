import {HttpContextToken, HttpInterceptorFn, HttpRequest} from '@angular/common/http';
import {inject} from "@angular/core";
import {AuthService} from "../services/auth.service";
import {switchMap} from "rxjs/operators";

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authSvc = inject(AuthService);
  // whenever this HttpContextToken is attached to a request 
  // as we did to login request earlier
  // it means the user does not need to be authenticated 
  // so we don't attach authorization header
  if (req.context.get(IS_PUBLIC)) {
    return next(req);
    
  }
  if (authSvc.isAuthenticated()) {
    const authRequest = addAuthorizationHeader(req);
    console.log("login is invoked")

    return next(authRequest);
  } else {
    console.log("user is not authenticated")
    return next(req);
  }
};
const addAuthorizationHeader = (req: HttpRequest<any>) => {

  const token = localStorage.getItem('token');
  return req.clone({
    headers: req.headers.set('Authorization', `Bearer ${token}`)
  });
};
export const IS_PUBLIC = new HttpContextToken(() => false);
import {CanActivateFn, Router} from '@angular/router';
import {inject} from "@angular/core";
import {AuthService} from "../services/auth.service";

export const authGuard: CanActivateFn = (route, state) => {
  if (!inject(AuthService).isAuthenticated()) {
    inject(Router).navigate(['/login']);
    return false;
  }
  return true;
};
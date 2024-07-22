import {CanActivateFn, Router} from '@angular/router';
import {inject} from "@angular/core";
import {AuthService} from "../../services/auth.service";

export const authGuard: CanActivateFn = (route, state) => {
  if (!inject(AuthService).isAuthenticated()) {
    console.log("auth guard is invoked")
    inject(Router).navigate(['/login']);
    return false;
  }
  return true;
};
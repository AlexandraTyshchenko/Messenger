import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginPageComponent } from './core/auth/pages/login-page/login-page.component';
import { accountGuard, authGuard } from './core/auth/guards';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { RegisterPageComponent } from './core/auth/pages/register-page/register-page.component';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';
import { EmailConfirmationComponent } from './core/auth/pages/email-confirmation/email-confirmation.component';
import { SuccessEmailConfirmationComponent } from './pages/success-email-confirmation/success-email-confirmation.component';
import { ChatComponent } from './components/chat/chat.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  {
    path: 'home',
    component: HomePageComponent,
    canActivate: [authGuard],
  },
  {
    path: 'login',
    component: AuthPageComponent,
    data: { isLoginMode: true },
  },
  {
    path: 'register',
    component: AuthPageComponent,
    data: { isLoginMode: false },
  },
  {
    path: 'confirm',
    component: EmailConfirmationComponent,
  },
  { path: 'confirmemail', component: SuccessEmailConfirmationComponent },

];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule],
})
export class AppRoutingModule {}

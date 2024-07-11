import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginPageComponent } from './core/auth/login/pages/login-page/login-page.component';
import {
  HttpClientModule,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { JwtModule, JwtModuleOptions } from '@auth0/angular-jwt';
import { ConversationsComponent } from './pages/conversations/conversations.component';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { authInterceptor } from './core/auth/auth.interceptor';
import { AuthService } from './core/auth/services/auth.service';

const JWT_Module_Options: JwtModuleOptions = {
  config: {
    tokenGetter: () => localStorage.getItem('token'),
  },
};

@NgModule({
  declarations: [
    AppComponent,
    ConversationsComponent,
    HomePageComponent,
    ConversationsComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    LoginPageComponent,
    HttpClientModule,
    JwtModule.forRoot(JWT_Module_Options),
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor])),
    {
      provide: APP_INITIALIZER,
      useFactory: initializerFactory,
      multi: true,
      deps: [AuthService],
    },
  ],

  bootstrap: [AppComponent],
})
export class AppModule {}

export function initializerFactory(authService: AuthService) {
  return () => authService.refreshToken();
}

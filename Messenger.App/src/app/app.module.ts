import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginPageComponent } from './core/auth/pages/login-page/login-page.component';
import {
  HttpClientModule,
  provideHttpClient,
  withInterceptors,
} from '@angular/common/http';
import { JwtModule, JwtModuleOptions } from '@auth0/angular-jwt';
import { ConversationsComponent } from './pages/conversations/conversations.component';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthService } from './core/services/auth.service';
import { CommonModule } from '@angular/common';  
import { ReactiveFormsModule } from '@angular/forms';
import { RegisterPageComponent } from './core/auth/pages/register-page/register-page.component';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';
import { EmailConfirmationComponent } from './core/auth/pages/email-confirmation/email-confirmation.component';
import { SuccessEmailConfirmationComponent } from './pages/success-email-confirmation/success-email-confirmation.component';
import { ConversationItemComponent } from './components/conversation-item/conversation-item.component';
import { ScrollerModule } from 'primeng/scroller';
import { InfiniteScrollModule } from 'ngx-infinite-scroll';
import { HeaderComponent } from './components/header/header.component';
import { ChatComponent } from './components/chat/chat.component';
import { MessageComponent } from './components/message/message.component';
import { errorInterceptor } from './core/interceptors/error.interceptor';
import { ToastDirective } from './directives/toast.directive';

const JWT_Module_Options: JwtModuleOptions = {
  config: {
    tokenGetter: () => localStorage.getItem('token'),
  },
};

@NgModule({
  declarations: [
    AppComponent,
    HomePageComponent,
    LoginPageComponent,
    RegisterPageComponent,
    AuthPageComponent,
    EmailConfirmationComponent,
    SuccessEmailConfirmationComponent,
    ConversationsComponent,
    ConversationItemComponent,
    HeaderComponent,
    ChatComponent,
    MessageComponent,
    ToastDirective,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    HttpClientModule,
    CommonModule,
    ReactiveFormsModule, 
    ScrollerModule,
    InfiniteScrollModule,
    JwtModule.forRoot(JWT_Module_Options),
  ],
  providers: [
    provideHttpClient(withInterceptors([authInterceptor,errorInterceptor])),
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

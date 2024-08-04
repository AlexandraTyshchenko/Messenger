import { APP_INITIALIZER, NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginPageComponent } from './core/auth/pages/login-page/login-page.component';
import { JwtModule, JwtModuleOptions } from '@auth0/angular-jwt';
import { ConversationsComponent } from './pages/conversations/conversations.component';
import { HomePageComponent } from './pages/home-page/home-page.component';
import { authInterceptor } from './core/interceptors/auth.interceptor';
import { AuthService } from './core/services/auth.service';
import { CommonModule } from '@angular/common';  
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
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
import { ConversationWithLastMessageComponent } from './components/conversation-with-last-message/conversation-with-last-message.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { AddParticipantsComponent } from './components/add-participants/add-participants.component';
import { SearchUsersComponent } from './components/search-users/search-users.component';
import { provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { provideAnimationsAsync } from '@angular/platform-browser/animations/async';
import { ParticipantsListComponent } from './components/participants-list/participants-list.component';
import { DeleteParticipantConfirmationModalComponent } from './components/delete-participant-confirmation-modal/delete-participant-confirmation-modal.component';
import { LeaveConversationConfirmationModalComponent } from './components/leave-conversation-confirmation-modal/leave-conversation-confirmation-modal.component';
import { GroupConversationFormComponent } from './components/group-conversation-form/group-conversation-form.component';


const JWT_Module_Options: JwtModuleOptions = {
  config: {
    tokenGetter: () => localStorage.getItem('token'),
  },
};

@NgModule({ declarations: [
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
        ConversationWithLastMessageComponent,
        AddParticipantsComponent,
        SearchUsersComponent,
        ParticipantsListComponent,
        DeleteParticipantConfirmationModalComponent,
        LeaveConversationConfirmationModalComponent,
        GroupConversationFormComponent,
    ],
    bootstrap: [AppComponent], imports: [BrowserModule,
        AppRoutingModule,
        CommonModule,
        ReactiveFormsModule,
        ScrollerModule,
        InfiniteScrollModule,
        NgbModule,
        FormsModule,
        JwtModule.forRoot(JWT_Module_Options)], providers: [
        provideHttpClient(withInterceptors([authInterceptor, errorInterceptor])),
        {
            provide: APP_INITIALIZER,
            useFactory: initializerFactory,
            multi: true,
            deps: [AuthService],
        },
        provideHttpClient(withInterceptorsFromDi()),
        provideAnimationsAsync(),
    ] })
export class AppModule {}

export function initializerFactory(authService: AuthService) {
  return () => authService.refreshToken();
}

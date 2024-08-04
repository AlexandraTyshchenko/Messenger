import { AfterViewInit, Component, Inject, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit, AfterViewInit{
  constructor(public authService: AuthService
) {}
  imgUrl: string | null = null;
  title = 'Messenger.App';

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    if(this.authService.isAuthenticated()){
      this.imgUrl = this.authService.user()!.imgUrl;
    }
  }

  logout() {
    this.authService.logout();
  }
}

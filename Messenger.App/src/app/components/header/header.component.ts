import { Component, Inject, OnInit } from '@angular/core';
import { AuthService } from '../../core/services/auth.service';
import { IAuthServiceToken } from '../../core/tokens';
import { IAuthService } from '../../core/interfaces/auth.interface';

@Component({
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrl: './header.component.css',
})
export class HeaderComponent implements OnInit {
  constructor(@Inject(IAuthServiceToken) public authService: IAuthService
) {}
  imgUrl: string | null = null;
  title = 'Messenger.App';

  ngOnInit(): void {
    this.imgUrl = this.authService.user()!.imgUrl;
  }


  logout() {
    this.authService.logout();
  }
}

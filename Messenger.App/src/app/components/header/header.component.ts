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
  userName:string|null=null

  ngOnInit(): void {
  }

  ngAfterViewInit(): void {
    if(this.authService.isAuthenticated()){
      this.imgUrl = this.authService.user()!.imgUrl;
      this.userName = this.authService.user()!.name
    }
  }

  logout() {
    this.authService.logout();
  }
}

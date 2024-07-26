import { Component } from '@angular/core';
import { AuthService } from './core/services/auth.service';
import { SignalRService } from './core/services/signalr.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrl: './app.component.css',
})
export class AppComponent {
  constructor(private signalRService: SignalRService) {}

  ngOnInit(): void {
    this.signalRService.startConnection();
  }
}

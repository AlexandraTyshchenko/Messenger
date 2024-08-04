import { Component, OnInit } from '@angular/core';
import { SignalRService } from '../../core/services/signalr.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent implements OnInit{
 
  constructor( private signalRService: SignalRService) {
    
  }
 
  ngOnInit(): void {
    this.signalRService.startConnection();
  }
}

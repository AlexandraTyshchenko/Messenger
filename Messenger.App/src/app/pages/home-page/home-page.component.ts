import { Component, OnDestroy, OnInit } from '@angular/core';
import { SignalRService } from '../../core/services/signalr.service';

@Component({
  selector: 'app-home-page',
  templateUrl: './home-page.component.html',
  styleUrl: './home-page.component.css'
})
export class HomePageComponent implements OnInit,OnDestroy  {
 
  constructor( private signalRService: SignalRService) {
    
  }
 
  ngOnInit(): void {
    this.signalRService.startConnection();
  }

  ngOnDestroy(): void {
    this.signalRService.disconnect();
  }
}

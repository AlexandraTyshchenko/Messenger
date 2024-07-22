import { Component, Input, OnInit } from '@angular/core';
import { Message } from '../core/classes/message.model';

@Component({
  selector: 'app-message',
  templateUrl: './message.component.html',
  styleUrls: ['./message.component.css']
})
export class MessageComponent implements OnInit {
    @Input() myMessage=false;
    @Input() message!:Message;

    ngOnInit(): void {
    }
}

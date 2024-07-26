import { Directive, ElementRef, OnInit } from '@angular/core';
import { Toast } from 'bootstrap';

@Directive({
  selector: '.toast' 
})
export class ToastDirective implements OnInit {
  private toast!: Toast;

  constructor(private el: ElementRef) {}

  ngOnInit() {
    this.toast = new Toast(this.el.nativeElement);
  }

  show() {
    this.toast.show();
    setTimeout(() => {
      this.toast.hide();
    }, 5000);
  }

  hide() {
    this.toast.hide();
  }

  isClosed(): boolean {
    return !this.el.nativeElement.classList.contains('show');
  }
}

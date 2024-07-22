import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-success-email-confirmation',
  templateUrl: './success-email-confirmation.component.html',
  styleUrls: ['./success-email-confirmation.component.css'],
})
export class SuccessEmailConfirmationComponent implements OnInit {
  errorMessage: string | null = null;
  success = false;

  constructor(
    private route: ActivatedRoute,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.route.queryParams.subscribe((params) => {
      const email = params['email'];
      const token = params['token'];

      if (email && token) {
        this.confirmEmail(email, token);
      }
    });
  }

  confirmEmail(email: string, token: string) {
    this.authService.confirm(email, token).subscribe(
      (response) => {
        this.success = true;
      },
      (error) => {
        console.error('Error confirming email', error);
        this.success = false;
        this.errorMessage = error.message;
      }
    );
  }
}

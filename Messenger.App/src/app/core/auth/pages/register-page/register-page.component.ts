import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../services/auth.service';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';
import { UserRegistration } from '../../login/classes/register-user.class';

@Component({
  selector: 'app-register-page',
  templateUrl: './register-page.component.html',
  styleUrls: ['./register-page.component.css'],
})
export class RegisterPageComponent implements OnInit {
  registerForm!: FormGroup;
  errorMessage: string | null = null;

  constructor(private authSvc: AuthService, private formBuilder: FormBuilder, private router: Router) {}

  ngOnInit() {
    this.registerForm = this.formBuilder.group({
      firstName: ['', [Validators.required]],
      lastName: ['', [Validators.required]],
      userName: ['', [Validators.required]],
      password: ['', [Validators.required]],
      email: ['', [Validators.required]],
      phoneNumber: ['', [Validators.required]],
      bio: ['']
    });
  }

  onRegisterFormSubmitted() {
    if (this.registerForm.invalid) {
      return;
    }
    const registerData: UserRegistration = this.registerForm.value as UserRegistration;
    this.authSvc.register(registerData).pipe(
      tap(response => {
        console.log('Registration successful');
        this.router.navigate(['/confirm']);
      }),
      catchError(error => {
        this.errorMessage = error.message;
        return of(null);
      })
    ).subscribe();
  }
}

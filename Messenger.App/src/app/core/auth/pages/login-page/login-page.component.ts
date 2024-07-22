import { Component, DestroyRef, inject, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { AuthService } from '../../../services/auth.service';
import { Login } from '../../login/interfaces';
import { catchError, tap } from 'rxjs/operators';
import { of } from 'rxjs';
@Component({
  selector: 'app-login-page',
  templateUrl: './login-page.component.html',
  styleUrls: ['./login-page.component.css'],
})
export class LoginPageComponent implements OnInit {
  private readonly authSvc = inject(AuthService);
  private readonly formBuilder = inject(FormBuilder);
  private readonly destroyRef = inject(DestroyRef);
  loginForm!: FormGroup;
  errorMessage: string | null = null;

  ngOnInit() {
    this.loginForm = this.formBuilder.group({
      username: ['', [Validators.required]],
      password: ['', Validators.required]
    });
  }

  onLoginFormSubmitted() {
    if (!this.loginForm.valid) {
      return;
    }
      console.log("Form Submitted");
      this.errorMessage = null;
  
      this.authSvc.login(this.loginForm.value as Login).pipe(
        takeUntilDestroyed(this.destroyRef),
        catchError(error => {
          this.errorMessage = error;
          console.error('Login error', this.errorMessage);
          return of(null);
        }),
        tap(response => {
          if (response) {
            console.log('Login successful');
          } else {
            console.log('Login failed');
          }
        })
      ).subscribe();
    }}


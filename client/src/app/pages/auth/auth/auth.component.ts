import {Component, OnInit} from '@angular/core';
import {MatTab, MatTabGroup} from '@angular/material/tabs';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {BehaviorSubject, Observable} from 'rxjs';
import {MatCard} from '@angular/material/card';
import {AuthService} from '../../../services/auth.service';
import {LoginRequest} from '../../../api/models/login-request';
import {Router} from '@angular/router';
import {RegisterRequest} from '../../../api/models/register-request';
import {MatSnackBar} from '@angular/material/snack-bar';
import {CustomSnackbarComponent} from '../../../components/custom-snackbar/custom-snackbar.component';

@Component({
  selector: 'app-auth',
  imports: [
    MatTabGroup,
    MatTab,
    ReactiveFormsModule,
    MatFormField,
    MatLabel,
    MatInput,
    MatButton,
    MatCard,

  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss',
})
export class AuthComponent implements OnInit{
  loginForm!: FormGroup;
  registerForm!: FormGroup;

  private loginErrorSubject = new BehaviorSubject<string>('');
  private signupErrorSubject = new BehaviorSubject<string>('');

  loginError: Observable<string> = this.loginErrorSubject.asObservable();
  signupError: Observable<string> = this.signupErrorSubject.asObservable();

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router, private snackBar: MatSnackBar) {}

  ngOnInit(): void {
    this.initForms();
  }

  private initForms(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]]
    });

    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      fullName: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
    });
  }

  get email() { return this.loginForm.get('email') as FormControl; }
  get password() { return this.loginForm.get('password') as FormControl; }


  login(): void {
    if (this.loginForm.valid) {
      const request: LoginRequest = {
        email: this.loginForm.get('email')!.value,
        password: this.loginForm.get('password')!.value
      };

      this.authService.login(request).subscribe({
          next: () => {
            console.log(localStorage.getItem('auth_token'));
            this.router.navigate(['/home'])
            this.snackBar.openFromComponent(CustomSnackbarComponent, {
              data: { message: 'Sikeres Bejelentkezés', actionLabel: 'Rendben' },
              duration: 3000,
              horizontalPosition: 'center',
            });
          },
          error: (err) => this.loginErrorSubject.next(err.error.message)
        });
    }
  }

  signup(): void {
    if (this.registerForm.valid) {
      const request: RegisterRequest = {
        email: this.registerForm.get('email')!.value,
        fullName: this.registerForm.get('fullName')!.value,
        username: this.registerForm.get('username')!.value,
        password: this.registerForm.get('password')!.value,
      }

      this.authService.register(request).subscribe({
        next: () => {
          console.log(localStorage.getItem('auth_token'));
          this.router.navigate(['/home']);
          this.snackBar.openFromComponent(CustomSnackbarComponent, {
            data: { message: 'Sikeres Regisztráció', actionLabel: 'Rendben' },
            duration: 3000,
            horizontalPosition: 'center',
          });
        },
        error: (err) => this.loginErrorSubject.next(err.error.message)
      });
    }
  }
}

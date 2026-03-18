import {Component, OnInit} from '@angular/core';
import {MatTab, MatTabGroup} from '@angular/material/tabs';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {MatCard} from '@angular/material/card';
import {AuthService} from '../../../services/auth.service';
import {LoginRequest} from '../../../api/models/login-request';
import {Router} from '@angular/router';
import {RegisterRequest} from '../../../api/models/register-request';
import {MatSnackBar} from '@angular/material/snack-bar';
import {CustomSnackbarComponent} from '../../../components/custom-snackbar/custom-snackbar.component';
import {ConnectionService} from '../../../services/connection-service.service';
import {MatProgressBar} from '@angular/material/progress-bar';
import {ErrorDisplayComponent} from '../../../components/error-display/error-display.component';
import {HttpValidationError} from '../../../services/error-interceptor.service';

@Component({
  selector: 'app-auth',
  imports: [
    MatTabGroup, MatTab, ReactiveFormsModule,
    MatFormField, MatLabel, MatInput, MatError,
    MatButton, ErrorDisplayComponent, MatCard, MatProgressBar,
  ],
  templateUrl: './auth.component.html',
  styleUrl: './auth.component.scss',
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  registerForm!: FormGroup;

  isServerLoading = true;
  isServerDown = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private router: Router,
    private snackBar: MatSnackBar,
    private connectionService: ConnectionService
  ) {}

  ngOnInit(): void {
    this.initForms();
    this.checkConnection();
  }

  checkConnection(): void {
    this.isServerLoading = true;
    this.isServerDown = false;
    this.connectionService.checkConnection().subscribe({
      next: () => { this.isServerLoading = false; this.isServerDown = false; },
      error: () => { this.isServerLoading = false; this.isServerDown = true; }
    });
  }

  private initForms(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.maxLength(50)]],
      fullName: ['', [Validators.required, Validators.maxLength(100)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
    });

    [this.loginEmail, this.loginPassword,
      this.regUsername, this.regFullName, this.regEmail, this.regPassword]
      .forEach(c => this.clearServerErrorOnChange(c));
  }

  // Login getters
  get loginEmail() { return this.loginForm.get('email') as FormControl; }
  get loginPassword() { return this.loginForm.get('password') as FormControl; }

  // Register getters
  get regUsername() { return this.registerForm.get('username') as FormControl; }
  get regFullName() { return this.registerForm.get('fullName') as FormControl; }
  get regEmail() { return this.registerForm.get('email') as FormControl; }
  get regPassword() { return this.registerForm.get('password') as FormControl; }

  // Szerver errorokat a megfelelő FormControl-ra rakja
  private applyServerErrors(form: FormGroup, error: HttpValidationError): void {
    const fieldMap: { [key: string]: string } = {
      // backend field neve -> FormControl neve
      'email': 'email',
      'password': 'password',
      'username': 'username',
      'fullname': 'fullName',
    };

    let hasFieldError = false;
    for (const backendField of Object.keys(error.fieldErrors)) {
      const controlName = fieldMap[backendField] ?? backendField;
      const control = form.get(controlName);
      if (control) {
        control.setErrors({ serverError: error.fieldErrors[backendField][0] });
        hasFieldError = true;
      }
    }

    // Ha nincs field-specifikus hiba, globális hiba az email alá kerül
    if (!hasFieldError && error.title) {
      form.get('email')?.setErrors({ serverError: error.title });
    }
  }

  login(): void {
    if (this.loginForm.invalid) return;

    const request: LoginRequest = {
      email: this.loginEmail.value,
      password: this.loginPassword.value
    };

    this.authService.login(request).subscribe({
      next: () => {
        this.router.navigate(['/home']);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Sikeres Bejelentkezés', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      },
      error: (err: HttpValidationError) => {
        this.applyServerErrors(this.loginForm, err)
      }
    });
  }

  signup(): void {
    if (this.registerForm.invalid) return;

    const request: RegisterRequest = {
      email: this.regEmail.value,
      fullName: this.regFullName.value,
      username: this.regUsername.value,
      password: this.regPassword.value,
    };

    this.authService.register(request).subscribe({
      next: () => {
        this.router.navigate(['/home']);
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Sikeres Regisztráció', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
      },
      error: (err: HttpValidationError) => this.applyServerErrors(this.registerForm, err)
    });
  }

  private clearServerErrorOnChange(control: FormControl): void {
    control.valueChanges.subscribe(() => {
      if (control.hasError('serverError')) {
        const errors = { ...control.errors };
        delete errors['serverError'];
        control.setErrors(Object.keys(errors).length ? errors : null);
      }
    });
  }

}

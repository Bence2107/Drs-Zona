import {Component, Input, OnInit} from '@angular/core';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {FormBuilder, FormControl, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatCard} from '@angular/material/card';
import {MatTab, MatTabGroup} from '@angular/material/tabs';
import {MatError, MatFormField, MatInput, MatLabel, MatSuffix} from '@angular/material/input';
import {MatSnackBar} from '@angular/material/snack-bar';
import {AuthService} from '../../../../../../services/api/auth.service';
import {ChangePasswordRequest} from '../../../../../../api/models/change-password-request';
import {CustomSnackbarComponent} from '../../../../../../components/custom-snackbar/custom-snackbar.component';
import {Router} from '@angular/router';
import {HttpValidationError} from '../../../../../../services/error-interceptor.service';

@Component({
  selector: 'app-profile-edit',
  imports: [
    MatIcon, MatCard, MatTabGroup, MatTab,
    ReactiveFormsModule, MatFormField, MatInput,
    MatLabel, MatButton, MatError, MatSuffix
  ],
  templateUrl: './profile-edit.component.html',
  styleUrl: './profile-edit.component.scss',
})
export class ProfileEditComponent implements OnInit {
  @Input() userData: UserProfileResponse | null = null;

  profileForm!: FormGroup;
  passwordForm!: FormGroup;
  isLoading = false;

  constructor(
    private fb: FormBuilder,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private router: Router
  ) {}

  ngOnInit() {
    this.profileForm = this.fb.group({
      fullName: [this.userData?.fullName || '', [Validators.required, Validators.maxLength(100)]],
      username: [this.userData?.username || '', [Validators.required, Validators.maxLength(50)]],
      email: [this.userData?.email || '', [Validators.required, Validators.email]]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required, Validators.minLength(8)]]
    });

    this.clearServerErrorOnChange([
      this.profFullName, this.profUsername, this.profEmail,
      this.passCurrentPassword, this.passNewPassword, this.passConfirmPassword
    ]);
  }

  get profFullName() { return this.profileForm.get('fullName') as FormControl; }
  get profUsername() { return this.profileForm.get('username') as FormControl; }
  get profEmail() { return this.profileForm.get('email') as FormControl; }

  get passCurrentPassword() { return this.passwordForm.get('currentPassword') as FormControl; }
  get passNewPassword() { return this.passwordForm.get('newPassword') as FormControl; }
  get passConfirmPassword() { return this.passwordForm.get('confirmPassword') as FormControl; }

  forceRefresh() {
    const currentUrl = this.router.url;
    this.router.navigateByUrl('/', { skipLocationChange: true }).then(() => {
      this.router.navigate([currentUrl]);
    });
  }

  onUpdateProfile() {
    if (this.profileForm.invalid) return;

    this.isLoading = true;
    this.authService.updateProfile(this.profileForm.value).subscribe({
      next: () => {
        this.forceRefresh();
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Profil sikeresen módosítva', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.isLoading = false;
      },
      error: (err: HttpValidationError) => {
        this.applyServerErrors(this.profileForm, err);
        this.isLoading = false;
      }
    });
  }

  onUpdatePassword() {
    if (this.passwordForm.invalid) return;

    const { newPassword, confirmPassword } = this.passwordForm.value;
    if (newPassword !== confirmPassword) {
      this.passConfirmPassword.setErrors({ serverError: 'A két jelszó nem egyezik' });
      return;
    }

    this.isLoading = true;
    const request: ChangePasswordRequest = {
      currentPassword: this.passCurrentPassword.value,
      newPassword: this.passNewPassword.value,
      newPasswordAgain: this.passConfirmPassword.value
    };

    this.authService.changePassword(request).subscribe({
      next: () => {
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Jelszó sikeresen módosítva', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.passwordForm.reset();
        this.isLoading = false;
      },
      error: (err: HttpValidationError) => {
        this.applyServerErrors(this.passwordForm, err);
        this.isLoading = false;
      }
    });
  }

  private clearServerErrorOnChange(controls: FormControl[]): void {
    controls.forEach(control => {
      control.valueChanges.subscribe(() => {
        if (control.hasError('serverError')) {
          const errors = { ...control.errors };
          delete errors['serverError'];
          control.setErrors(Object.keys(errors).length ? errors : null);
        }
      });
    });
  }

  private applyServerErrors(form: FormGroup, error: HttpValidationError): void {
    if (!error?.fieldErrors) {
      form.get('email')?.setErrors({ serverError: error?.title ?? 'Ismeretlen hiba' });
      return;
    }

    const fieldMap: { [key: string]: string } = {
      'email': 'email',
      'username': 'username',
      'fullname': 'fullName',
      'currentpassword': 'currentPassword',
      'newpassword': 'newPassword',
      'newpasswordagain': 'confirmPassword',
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

    if (!hasFieldError && error.title) {
      form.get('email')?.setErrors({ serverError: error.title });
    }
  }
}

import {Component, Input, OnInit} from '@angular/core';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatCard} from '@angular/material/card';
import {MatTab, MatTabGroup} from '@angular/material/tabs';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatSnackBar} from '@angular/material/snack-bar';
import {AuthService} from '../../../../../../services/auth.service';
import {ChangePasswordRequest} from '../../../../../../api/models/change-password-request';
import {CustomSnackbarComponent} from '../../../../../../components/custom-snackbar/custom-snackbar.component';
import {Router} from '@angular/router';

@Component({
  selector: 'app-profile-edit',
  imports: [
    MatIcon,
    MatCard,
    MatTabGroup,
    MatTab,
    ReactiveFormsModule,
    MatFormField,
    MatInput,
    MatLabel,
    MatButton
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
      fullName: [this.userData?.fullName || '', Validators.required],
      username: [this.userData?.username || '', Validators.required],
      email: [this.userData?.email || '', [Validators.required, Validators.email]]
    });

    this.passwordForm = this.fb.group({
      currentPassword: ['', Validators.required],
      newPassword: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    });
  }

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
        this.forceRefresh()
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Profil sikeresen módosítva', actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.isLoading = false;
      },
      error: (err) => {
        this.snackBar.open('Hiba történt a mentés során: ' + (err.error?.message || 'Ismeretlen hiba'), 'Bezár', { duration: 5000 });
        this.isLoading = false;
      }
    });
  }

  onUpdatePassword() {
    if (this.passwordForm.invalid) return;

    const { currentPassword, newPassword, confirmPassword } = this.passwordForm.value;

    if (newPassword !== confirmPassword) {
      this.snackBar.open('A két új jelszó nem egyezik!', 'Hiba', { duration: 3000 });
      return;
    }

    this.isLoading = true;
    const request: ChangePasswordRequest = {
      currentPassword: currentPassword,
      newPassword: newPassword,
      newPasswordAgain: confirmPassword
    }
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
      error: (err) => {
        this.snackBar.openFromComponent(CustomSnackbarComponent, {
          data: { message: 'Sikertelen jelszómódosítás: ' + err, actionLabel: 'Rendben' },
          duration: 3000,
          horizontalPosition: 'center',
        });
        this.isLoading = false;
      }
    });
  }
}

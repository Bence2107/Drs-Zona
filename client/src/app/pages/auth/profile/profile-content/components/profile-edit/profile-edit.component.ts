import {Component, Input, OnInit} from '@angular/core';
import {UserProfileResponse} from '../../../../../../api/models/user-profile-response';
import {FormBuilder, FormGroup, ReactiveFormsModule, Validators} from '@angular/forms';
import {MatIcon} from '@angular/material/icon';
import {MatButton} from '@angular/material/button';
import {MatCard} from '@angular/material/card';
import {MatTab, MatTabGroup} from '@angular/material/tabs';
import {MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {AuthService} from '../../../../../../services/auth.service';

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
  previewUrl: string | null = null;

  constructor(private fb: FormBuilder, private authService: AuthService) {}

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

  handleFile(event: any) {
    const file = event.target.files[0];
    if (file) {
      const reader = new FileReader();
      reader.onload = () => this.previewUrl = reader.result as string;
      reader.readAsDataURL(file);
    }
  }

  onUpdateProfile() {
    console.log('Adatok küldése a szervernek:', this.profileForm.value);
  }

  onUpdatePassword() {
    if(this.passwordForm.value.newPassword !== this.passwordForm.value.confirmPassword) {
      alert('A két jelszó nem egyezik!');
      return;
    }
    console.log('Jelszó módosítás...');
  }
}

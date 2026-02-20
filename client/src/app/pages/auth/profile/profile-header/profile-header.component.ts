import { Component, Input } from '@angular/core';
import { MatCard } from '@angular/material/card';
import { MatIcon } from '@angular/material/icon';
import { AuthService } from '../../../../services/auth.service';
import { UserProfileResponse } from '../../../../api/models/user-profile-response';
import {MatTooltip} from '@angular/material/tooltip';

@Component({
  selector: 'app-profile-header',
  standalone: true,
  imports: [MatCard, MatIcon, MatTooltip],
  templateUrl: './profile-header.component.html',
  styleUrl: './profile-header.component.scss',
})
export class ProfileHeaderComponent {
  @Input() userData: UserProfileResponse | null = null;

  constructor(private authService: AuthService) {}

  get avatarUrl(): string {
    const profile = this.authService.currentProfile();
    if (!profile?.hasAvatar || !profile?.avatarUrl) return "img/user/avatars/avatar.jpg";
    return `${profile.avatarUrl}`;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files[0]) {
      const file = input.files[0];
      console.log('Upload target:', file);
    }
  }
}

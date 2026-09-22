import {Component, OnInit} from '@angular/core';
import {ProfileHeaderComponent} from './profile-header/profile-header.component';
import {ProfileContentComponent} from './profile-content/profile-content.component';
import {UserProfileResponse} from '../../../api/models/user-profile-response';
import {AuthService} from '../../../services/api/auth.service';
import {ImagePreloadService} from '../../../services/image-preload.service';

@Component({
  selector: 'app-profile',
  imports: [
    ProfileHeaderComponent,
    ProfileContentComponent
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss',
})
export class ProfileComponent implements OnInit{
  userData: UserProfileResponse | null = null;
  isLoading = true;

  constructor(
    private authService: AuthService,
    private imagePreload: ImagePreloadService,
  ) {}

  ngOnInit() {
    this.authService.getMe()?.subscribe(data => {
      this.userData = data;
      this.imagePreload
        .preload([this.avatarUrl, 'img/user/profile/background.jpg'])
        .then(() => this.isLoading = false);
    });
  }

  get avatarUrl(): string {
    const profile = this.authService.currentProfile();
    if (!profile?.hasAvatar || !profile?.avatarUrl) return "img/user/avatars/avatar.jpg";
    return `${profile.avatarUrl}`;
  }
}

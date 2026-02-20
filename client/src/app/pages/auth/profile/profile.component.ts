import {Component, OnInit} from '@angular/core';
import {ProfileHeaderComponent} from './profile-header/profile-header.component';
import {ProfileContentComponent} from './profile-content/profile-content.component';
import {UserProfileResponse} from '../../../api/models/user-profile-response';
import {AuthService} from '../../../services/auth.service';

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

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.authService.getMe()?.subscribe(data => {
      this.userData = data;
    });
  }
}

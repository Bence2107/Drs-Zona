import {Component, Input} from '@angular/core';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {UserProfileResponse} from '../../../../api/models/user-profile-response';
import {AuthService} from '../../../../services/auth.service';
import {MatIcon} from '@angular/material/icon';
import {ProfileGeneralComponent} from './components/profile-general/profile-general.component';
import {ProfileEditComponent} from './components/profile-edit/profile-edit.component';
import {ProfileCommentsComponent} from './components/profile-comments/profile-comments.component';

@Component({
  selector: 'app-profile-content',
  imports: [
    MatButtonToggleGroup,
    MatButtonToggle,
    MatIcon,
    ProfileGeneralComponent,
    ProfileEditComponent,
    ProfileCommentsComponent
  ],
  templateUrl: './profile-content.component.html',
  styleUrl: './profile-content.component.scss',
})
export class ProfileContentComponent {
  @Input() userData: UserProfileResponse | null = null;
  activeTab = 'general';
  @Input() avatarUrl: string | null = null;

  constructor(private authService: AuthService) {}

  onTabChange(view: string) {
    this.activeTab = view;
  }
}

import {Component, inject, Input, OnInit} from '@angular/core';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {UserProfileResponse} from '../../../../api/models/user-profile-response';
import {AuthService} from '../../../../services/auth.service';
import {MatIcon} from '@angular/material/icon';
import {ProfileGeneralComponent} from './components/profile-general/profile-general.component';
import {ProfileEditComponent} from './components/profile-edit/profile-edit.component';
import {ProfileCommentsComponent} from './components/profile-comments/profile-comments.component';
import {map} from 'rxjs/operators';
import {toSignal} from '@angular/core/rxjs-interop';
import {BreakpointObserver} from '@angular/cdk/layout';
import {MatFormField} from '@angular/material/input';
import {MatOption, MatSelect} from '@angular/material/select';

@Component({
  selector: 'app-profile-content',
  imports: [
    MatButtonToggleGroup,
    MatButtonToggle,
    MatIcon,
    ProfileGeneralComponent,
    ProfileEditComponent,
    ProfileCommentsComponent,
    MatFormField,
    MatSelect,
    MatOption
  ],
  templateUrl: './profile-content.component.html',
  styleUrl: './profile-content.component.scss',
})
export class ProfileContentComponent implements OnInit {
  ngOnInit(): void {
      this.activeTab = 'general';
  }
  @Input() userData: UserProfileResponse | null = null;
  @Input() avatarUrl: string | null = null;

  private breakpointObserver = inject(BreakpointObserver);

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 768px)').pipe(
      map(result => result.matches)
    )
  );

  isTablet = toSignal(
    this.breakpointObserver.observe('(min-width: 769px) and (max-width: 1242px)').pipe(
      map(result => result.matches)
    )
  );

  activeTab = 'general';

  onTabChange(view: string) {
    this.activeTab = view;
  }
}

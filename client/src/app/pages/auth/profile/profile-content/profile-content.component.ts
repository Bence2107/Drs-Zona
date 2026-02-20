import {Component, inject, Input, OnDestroy, OnInit} from '@angular/core';
import {MatButtonToggle, MatButtonToggleGroup} from '@angular/material/button-toggle';
import {UserProfileResponse} from '../../../../api/models/user-profile-response';
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
export class ProfileContentComponent implements OnInit, OnDestroy {
  @Input() userData: UserProfileResponse | null = null;
  @Input() avatarUrl: string | null = null;

  private breakpointObserver = inject(BreakpointObserver);
  private readonly TAB_KEY = 'profile_active_tab';

  isMobile = toSignal(
    this.breakpointObserver.observe('(max-width: 768px)').pipe(
      map(result => result.matches)
    )
  );

  activeTab = 'general';

  ngOnInit(): void {
    const savedTab = sessionStorage.getItem(this.TAB_KEY);
    if (savedTab) {
      this.activeTab = savedTab;
    }
  }

  ngOnDestroy(): void {
    sessionStorage.removeItem(this.TAB_KEY);
  }

  onTabChange(view: string) {
    this.activeTab = view;
    sessionStorage.setItem(this.TAB_KEY, view);
  }
}

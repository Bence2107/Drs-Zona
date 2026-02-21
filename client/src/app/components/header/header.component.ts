import {Component, EventEmitter, OnInit, Output} from '@angular/core';
import {BreakpointObserver} from '@angular/cdk/layout';
import {MatButton, MatIconButton} from '@angular/material/button';

import {MatIcon} from '@angular/material/icon';
import {RouterLink, RouterLinkActive} from '@angular/router';
import {MatTooltip} from '@angular/material/tooltip';
import {MatSlideToggle} from '@angular/material/slide-toggle';
import {AuthService} from '../../services/auth.service';
import {MatMenu, MatMenuItem, MatMenuTrigger} from '@angular/material/menu';
import {MatDivider} from '@angular/material/list';
import {CustomSnackbarComponent} from '../custom-snackbar/custom-snackbar.component';
import {MatSnackBar} from '@angular/material/snack-bar';
import {ConnectionService} from '../../services/connection-service.service';
import {MatProgressSpinner} from '@angular/material/progress-spinner';

@Component({
  selector: 'app-header',
  imports: [
    MatIcon,
    RouterLink,
    RouterLinkActive,
    MatButton,
    MatTooltip,
    MatSlideToggle,
    MatIconButton,
    MatMenu,
    MatMenuItem,
    MatDivider,
    MatMenuTrigger,
    MatProgressSpinner
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  isDarkMode: boolean = localStorage.getItem('theme') === 'dark';

  @Output() toggleSidenav = new EventEmitter<void>();
  isScreenSmall: boolean = false;

  isAuthLoading: boolean = true;
  isServerAlive: boolean = false;
  isAvatarLoading: boolean = true;

  constructor(
    private breakpointObserver: BreakpointObserver,
    private authService: AuthService,
    private snackBar: MatSnackBar,
    private connectionService: ConnectionService) {
    this.breakpointObserver.observe(['(max-width: 990px)']).subscribe(result => {
      this.isScreenSmall = result.matches;
    });
  }

  onAvatarLoad() {
    this.isAvatarLoading = false;
  }

  onAvatarError() {
    this.isAvatarLoading = false;
  }

  get showSpinner(): boolean {
    return this.isAuthLoading || (this.canShowUserMenu() && this.isAvatarLoading);
  }

  canShowUserMenu(): boolean {
    return !this.isAuthLoading && this.isLoggedIn() && this.isServerAlive && !this.isScreenSmall && this.hasProfileData;
  }

  ngOnInit(): void {
    this.initializeTheme();
    this.verifyServerAndAuth();
  }

  private verifyServerAndAuth(): void {
    this.connectionService.checkConnection().subscribe({
      next: () => {
        this.isServerAlive = true;
        this.isAuthLoading = false;

        if (!this.isLoggedIn()) {
          this.isAvatarLoading = false;
        }
      },
      error: () => {
        this.isServerAlive = false;
        this.isAuthLoading = false;
        this.isAvatarLoading = false;
      }
    });
  }

  toggleSidebar() {
    this.toggleSidenav.emit();
  }

  private initializeTheme(): void {
    const savedTheme = localStorage.getItem('theme') ?? 'light';
    document.documentElement.setAttribute('data-theme', savedTheme);
    this.isDarkMode = savedTheme === 'dark';
  }

  toggleTheme() {
    this.isDarkMode = !this.isDarkMode;
    const theme = this.isDarkMode ? 'dark' : 'light';
    localStorage.setItem('theme', theme);
    document.documentElement.setAttribute('data-theme', theme);
  }

  isLoggedIn() {
    return this.authService.isLoggedIn();
  }

  logout() {
    this.snackBar.openFromComponent(CustomSnackbarComponent, {
      data: { message: 'Kijelentkezve', actionLabel: 'Rendben' },
      duration: 3000,
      horizontalPosition: 'center',
    });
    return this.authService.logout();
  }

  get username(): string | null {
    return this.authService.currentProfile()?.username ?? null;
  }

  get hasProfileData(): boolean {
    return !!this.authService.currentProfile();
  }

  get avatarUrl(): string {
    const profile = this.authService.currentProfile();
    if (profile?.hasAvatar && profile?.avatarUrl) {
      return profile.avatarUrl;
    }
    return "img/user/avatars/avatar.jpg";
  }

  isAuthor(): boolean {
    return this.authService.currentProfile()?.role === 'Author';
  }
}

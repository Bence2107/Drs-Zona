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
    MatMenuTrigger
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent implements OnInit {
  isDarkMode: boolean = localStorage.getItem('theme') === 'dark';

  @Output() toggleSidenav = new EventEmitter<void>();
  isScreenSmall: boolean = false;

  constructor(private breakpointObserver: BreakpointObserver, private authService: AuthService) {
    this.breakpointObserver.observe(['(max-width: 990px)']).subscribe(result => {
      this.isScreenSmall = result.matches;
    });
  }

  ngOnInit(): void {
    this.initializeTheme();
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
    return this.authService.logout();
  }

  get username(): string | null {
    return this.authService.currentProfile()?.username ?? null;
  }

  get avatarUrl(): string | null {
    const profile = this.authService.currentProfile();
    if (!profile?.hasAvatar || !profile?.avatarUrl) return "img/user/avatars/avatar.jpg";
    return `${profile.avatarUrl}`;
  }

  isAuthor(): boolean {
    return this.authService.currentProfile()?.role === 'Author';
  }
}
